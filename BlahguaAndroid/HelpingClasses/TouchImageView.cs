/*
 * TouchImageView.java
 * By: Michael Ortiz
 * Updated By: Patrick Lackemacher
 * Updated By: Babay88
 * Updated By: @ipsilondev
 * Updated By: hank-cp
 * Updated By: singpolyma
 * -------------------
 * Extends Android ImageView to include pinch zooming, panning, fling and double tap zoom.
 * 
 * C# port by: Dmitriy Goliy
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Util;
using Java.Lang;
using Android.Views.Animations;
using Android.Graphics.Drawables;
using Android.Content.Res;

namespace BlahguaMobile.AndroidClient.HelpingClasses
{
    class TouchImageView : ImageView
    {
	
	private static readonly string DEBUG = "DEBUG";
	
	//
	// SuperMin and SuperMax multipliers. Determine how much the image can be
	// zoomed below or above the zoom boundaries, before animating back to the
	// min/max zoom boundary.
	//
	private static readonly float SUPER_MIN_MULTIPLIER = .75f;
	private static readonly float SUPER_MAX_MULTIPLIER = 1.25f;

    //
    // Scale of image ranges from minScale to maxScale, where minScale == 1
    // when the image is stretched to fit view.
    //
    private float normalizedScale;
    
    //
    // Matrix applied to image. MSCALE_X and MSCALE_Y should always be equal.
    // MTRANS_X and MTRANS_Y are the other values used. prevMatrix is the matrix
    // saved prior to the screen rotating.
    //
	private Matrix matrix, prevMatrix;

    private enum State { NONE, DRAG, ZOOM, FLING, ANIMATE_ZOOM };
    private State state;

    private float minScale;
    private float maxScale;
    private float superMinScale;
    private float superMaxScale;
    private float[] m;
    
    private Context context;
    private Fling fling;
    
    private ScaleType mScaleType;
    
    private bool imageRenderedAtLeastOnce;
    private bool onDrawReady;
    
    private ZoomVariables delayedZoomVariables;

    //
    // Size of view and previous view size (ie before rotation)
    //
    private int viewWidth, viewHeight, prevViewWidth, prevViewHeight;
    
    //
    // Size of image when it is stretched to fit view. Before and After rotation.
    //
    private float matchViewWidth, matchViewHeight, prevMatchViewWidth, prevMatchViewHeight;
    
    private ScaleGestureDetector mScaleDetector;
    private GestureDetector mGestureDetector;
    private GestureDetector.IOnDoubleTapListener doubleTapListener = null;
    private IOnTouchListener userTouchListener = null;
    private OnTouchImageViewListener touchImageViewListener = null;

    public TouchImageView(Context context) : base(context) {
        SharedConstructing(context);
    }

    public TouchImageView(Context context, IAttributeSet attrs) : base(context, attrs)  {
        SharedConstructing(context);
    }
    
    public TouchImageView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)  {
    	SharedConstructing(context);
    }
    
    private void SharedConstructing(Context context) {
        base.Clickable = true;
        
        this.context = context;
        mScaleDetector = new ScaleGestureDetector(context, new ScaleListener(this));
        mGestureDetector = new GestureDetector(context, new GestureListener(this));
        matrix = new Matrix();
        prevMatrix = new Matrix();
        m = new float[9];
        normalizedScale = 1;
        if (mScaleType == null) {
        	mScaleType = ScaleType.FitCenter;
        }
        minScale = 1;
        maxScale = 3;
        superMinScale = SUPER_MIN_MULTIPLIER * minScale;
        superMaxScale = SUPER_MAX_MULTIPLIER * maxScale;
        ImageMatrix = matrix;
        SetScaleType(ScaleType.Matrix);
        SetState(State.NONE);
        onDrawReady = false;
        base.SetOnTouchListener(new PrivateOnTouchListener(this));
    }

    public void SetOnTouchListener(View.IOnTouchListener l) {
        userTouchListener = l;
    }
    
    public void SetOnTouchImageViewListener(OnTouchImageViewListener l) {
    	touchImageViewListener = l;
    }

    public void SetOnDoubleTapListener(GestureDetector.IOnDoubleTapListener l) {
        doubleTapListener = l;
    }
    
    public void SetImageResource(int resId) {
    	base.SetImageResource(resId);
    	SavePreviousImageValues();
    	FitImageToView();
    }
    
    public void SetImageBitmap(Bitmap bm) {
    	base.SetImageBitmap(bm);
        SavePreviousImageValues();
    	FitImageToView();
    }
    
    public void SetImageDrawable(Drawable drawable) {
    	base.SetImageDrawable(drawable);
    	SavePreviousImageValues();
    	FitImageToView();
    }
    
    public void SetImageURI(Android.Net.Uri uri) {
    	base.SetImageURI(uri);
    	SavePreviousImageValues();
    	FitImageToView();
    }
    
    public void SetScaleType(ScaleType type) {
    	if (type == ScaleType.FitStart || type == ScaleType.FitEnd) {
    		throw new UnsupportedOperationException("TouchImageView does not support FIT_START or FIT_END");
    	}
    	if (type == ScaleType.Matrix) {
    		base.SetScaleType(ScaleType.Matrix);
    		
    	} else {
    		mScaleType = type;
    		if (onDrawReady) {
    			//
    			// If the image is already rendered, scaleType has been called programmatically
    			// and the TouchImageView should be updated with the new scaleType.
    			//
    			SetZoom(this);
    		}
    	}
    }
    
    public ScaleType GetScaleType() {
    	return mScaleType;
    }
    
    /**
     * Returns false if image is in initial, unzoomed state. False, otherwise.
     * @return true if image is zoomed
     */
    public bool IsZoomed() {
    	return normalizedScale != 1;
    }
    
    /**
     * Return a Rect representing the zoomed image.
     * @return rect representing zoomed image
     */
    public RectF GetZoomedRect() {
    	if (mScaleType == ScaleType.FitXy) {
    		throw new UnsupportedOperationException("getZoomedRect() not supported with FIT_XY");
    	}
    	PointF topLeft = TransformCoordTouchToBitmap(0, 0, true);
    	PointF bottomRight = TransformCoordTouchToBitmap(viewWidth, viewHeight, true);
    	
    	float w = Drawable.IntrinsicWidth;
    	float h = Drawable.IntrinsicHeight;
    	return new RectF(topLeft.X / w, topLeft.Y / h, bottomRight.X / w, bottomRight.Y / h);
    }
    
    /**
     * Save the current matrix and view dimensions
     * in the prevMatrix and prevView variables.
     */
    private void SavePreviousImageValues() {
    	if (matrix != null && viewHeight != 0 && viewWidth != 0) {
	    	matrix.GetValues(m);
	    	prevMatrix.SetValues(m);
	    	prevMatchViewHeight = matchViewHeight;
	        prevMatchViewWidth = matchViewWidth;
	        prevViewHeight = viewHeight;
	        prevViewWidth = viewWidth;
    	}
    }

    public IParcelable OnSaveInstanceState()
    {
    	Bundle bundle = new Bundle();
    	bundle.PutParcelable("instanceState", base.OnSaveInstanceState());
    	bundle.PutFloat("saveScale", normalizedScale);
    	bundle.PutFloat("matchViewHeight", matchViewHeight);
    	bundle.PutFloat("matchViewWidth", matchViewWidth);
    	bundle.PutInt("viewWidth", viewWidth);
    	bundle.PutInt("viewHeight", viewHeight);
    	matrix.GetValues(m);
    	bundle.PutFloatArray("matrix", m);
    	bundle.PutBoolean("imageRendered", imageRenderedAtLeastOnce);
    	return bundle;
    }

    public void OnRestoreInstanceState(IParcelable state)
    {
      	if (state is Bundle) {
	        Bundle bundle = (Bundle) state;
	        normalizedScale = bundle.GetFloat("saveScale");
	        m = bundle.GetFloatArray("matrix");
	        prevMatrix.SetValues(m);
	        prevMatchViewHeight = bundle.GetFloat("matchViewHeight");
	        prevMatchViewWidth = bundle.GetFloat("matchViewWidth");
	        prevViewHeight = bundle.GetInt("viewHeight");
	        prevViewWidth = bundle.GetInt("viewWidth");
	        imageRenderedAtLeastOnce = bundle.GetBoolean("imageRendered");
	        base.OnRestoreInstanceState((IParcelable)bundle.GetParcelable("instanceState"));
	        return;
      	}

      	base.OnRestoreInstanceState(state);
    }

    protected override void OnDraw(Canvas canvas)
    {
    	onDrawReady = true;
    	imageRenderedAtLeastOnce = true;
    	if (delayedZoomVariables != null) {
    		SetZoom(delayedZoomVariables.scale, delayedZoomVariables.focusX, delayedZoomVariables.focusY, delayedZoomVariables.scaleType);
    		delayedZoomVariables = null;
    	}
    	base.OnDraw(canvas);
    }

    public void OnConfigurationChanged(Configuration newConfig)
    {
    	base.OnConfigurationChanged(newConfig);
    	SavePreviousImageValues();
    }
    
    /**
     * Get the max zoom multiplier.
     * @return max zoom multiplier.
     */
    public float GetMaxZoom() {
    	return maxScale;
    }

    /**
     * Set the max zoom multiplier. Default value: 3.
     * @param max max zoom multiplier.
     */
    public void SetMaxZoom(float max) {
        maxScale = max;
        superMaxScale = SUPER_MAX_MULTIPLIER * maxScale;
    }
    
    /**
     * Get the min zoom multiplier.
     * @return min zoom multiplier.
     */
    public float GetMinZoom() {
    	return minScale;
    }
    
    /**
     * Get the current zoom. This is the zoom relative to the initial
     * scale, not the original resource.
     * @return current zoom multiplier.
     */
    public float GetCurrentZoom() {
    	return normalizedScale;
    }
    
    /**
     * Set the min zoom multiplier. Default value: 1.
     * @param min min zoom multiplier.
     */
    public void SetMinZoom(float min) {
    	minScale = min;
    	superMinScale = SUPER_MIN_MULTIPLIER * minScale;
    }
    
    /**
     * Reset zoom and translation to initial state.
     */
    public void ResetZoom() {
    	normalizedScale = 1;
    	FitImageToView();
    }
    
    /**
     * Set zoom to the specified scale. Image will be centered by default.
     * @param scale
     */
    public void SetZoom(float scale) {
    	SetZoom(scale, 0.5f, 0.5f);
    }
    
    /**
     * Set zoom to the specified scale. Image will be centered around the point
     * (focusX, focusY). These floats range from 0 to 1 and denote the focus point
     * as a fraction from the left and top of the view. For example, the top left 
     * corner of the image would be (0, 0). And the bottom right corner would be (1, 1).
     * @param scale
     * @param focusX
     * @param focusY
     */
    public void SetZoom(float scale, float focusX, float focusY) {
    	SetZoom(scale, focusX, focusY, mScaleType);
    }
    
    /**
     * Set zoom to the specified scale. Image will be centered around the point
     * (focusX, focusY). These floats range from 0 to 1 and denote the focus point
     * as a fraction from the left and top of the view. For example, the top left 
     * corner of the image would be (0, 0). And the bottom right corner would be (1, 1).
     * @param scale
     * @param focusX
     * @param focusY
     * @param scaleType
     */
    public void SetZoom(float scale, float focusX, float focusY, ScaleType scaleType) {
    	//
    	// setZoom can be called before the image is on the screen, but at this point, 
    	// image and view sizes have not yet been calculated in onMeasure. Thus, we should
    	// delay calling setZoom until the view has been measured.
    	//
    	if (!onDrawReady) {
    		delayedZoomVariables = new ZoomVariables(scale, focusX, focusY, scaleType);
    		return;
    	}
    	
    	if (scaleType != mScaleType) {
    		SetScaleType(scaleType);
    	}
    	ResetZoom();
    	ScaleImage(scale, viewWidth / 2, viewHeight / 2, true);
    	matrix.GetValues(m);
    	m[Matrix.MtransX] = -((focusX * GetImageWidth()) - (viewWidth * 0.5f));
    	m[Matrix.MtransY] = -((focusY * GetImageHeight()) - (viewHeight * 0.5f));
    	matrix.SetValues(m);
    	FixTrans();
    	ImageMatrix = matrix;
    }
    
    /**
     * Set zoom parameters equal to another TouchImageView. Including scale, position,
     * and ScaleType.
     * @param TouchImageView
     */
    public void SetZoom(TouchImageView img) {
    	PointF center = img.GetScrollPosition();
    	SetZoom(img.GetCurrentZoom(), center.X, center.Y, img.GetScaleType());
    }
    
    /**
     * Return the point at the center of the zoomed image. The PointF coordinates range
     * in value between 0 and 1 and the focus point is denoted as a fraction from the left 
     * and top of the view. For example, the top left corner of the image would be (0, 0). 
     * And the bottom right corner would be (1, 1).
     * @return PointF representing the scroll position of the zoomed image.
     */
    public PointF GetScrollPosition() {
    	Drawable drawable = Drawable;
    	if (drawable == null) {
    		return null;
    	}
    	int drawableWidth = drawable.IntrinsicWidth;
        int drawableHeight = drawable.IntrinsicHeight;
        
        PointF point = TransformCoordTouchToBitmap(viewWidth / 2, viewHeight / 2, true);
        point.X /= drawableWidth;
        point.Y /= drawableHeight;
        return point;
    }
    
    /**
     * Set the focus point of the zoomed image. The focus points are denoted as a fraction from the
     * left and top of the view. The focus points can range in value between 0 and 1. 
     * @param focusX
     * @param focusY
     */
    public void SetScrollPosition(float focusX, float focusY) {
    	SetZoom(normalizedScale, focusX, focusY);
    }
    
    /**
     * Performs boundary checking and fixes the image matrix if it 
     * is out of bounds.
     */
    private void FixTrans() {
        matrix.GetValues(m);
        float transX = m[Matrix.MtransX];
        float transY = m[Matrix.MtransY];
        
        float fixTransX = GetFixTrans(transX, viewWidth, GetImageWidth());
        float fixTransY = GetFixTrans(transY, viewHeight, GetImageHeight());
        
        if (fixTransX != 0 || fixTransY != 0) {
            matrix.PostTranslate(fixTransX, fixTransY);
        }
    }
    
    /**
     * When transitioning from zooming from focus to zoom from center (or vice versa)
     * the image can become unaligned within the view. This is apparent when zooming
     * quickly. When the content size is less than the view size, the content will often
     * be centered incorrectly within the view. fixScaleTrans first calls fixTrans() and 
     * then makes sure the image is centered correctly within the view.
     */
    private void FixScaleTrans() {
    	FixTrans();
    	matrix.GetValues(m);
    	if (GetImageWidth() < viewWidth) {
    		m[Matrix.MtransX] = (viewWidth - GetImageWidth()) / 2;
    	}
    	
    	if (GetImageHeight() < viewHeight) {
    		m[Matrix.MtransY] = (viewHeight - GetImageHeight()) / 2;
    	}
    	matrix.SetValues(m);
    }

    private float GetFixTrans(float trans, float viewSize, float contentSize) {
        float minTrans, maxTrans;

        if (contentSize <= viewSize) {
            minTrans = 0;
            maxTrans = viewSize - contentSize;
            
        } else {
            minTrans = viewSize - contentSize;
            maxTrans = 0;
        }

        if (trans < minTrans)
            return -trans + minTrans;
        if (trans > maxTrans)
            return -trans + maxTrans;
        return 0;
    }
    
    private float GetFixDragTrans(float delta, float viewSize, float contentSize) {
        if (contentSize <= viewSize) {
            return 0;
        }
        return delta;
    }
    
    private float GetImageWidth() {
    	return matchViewWidth * normalizedScale;
    }
    
    private float GetImageHeight() {
    	return matchViewHeight * normalizedScale;
    }

    protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec) {
        Drawable drawable = Drawable;
        if (drawable == null || drawable.IntrinsicWidth == 0 || drawable.IntrinsicHeight == 0) {
        	SetMeasuredDimension(0, 0);
        	return;
        }
        
        int drawableWidth = drawable.IntrinsicWidth;
        int drawableHeight = drawable.IntrinsicHeight;
        int widthSize = MeasureSpec.GetSize(widthMeasureSpec);
        MeasureSpecMode widthMode = MeasureSpec.GetMode(widthMeasureSpec);
        int heightSize = MeasureSpec.GetSize(heightMeasureSpec);
        MeasureSpecMode heightMode = MeasureSpec.GetMode(heightMeasureSpec);
        viewWidth = SetViewSize(widthMode, widthSize, drawableWidth);
        viewHeight = SetViewSize(heightMode, heightSize, drawableHeight);
        
        //
        // Set view dimensions
        //
        SetMeasuredDimension(viewWidth, viewHeight);
        
        //
        // Fit content within view
        //
        FitImageToView();
    }
    
    /**
     * If the normalizedScale is equal to 1, then the image is made to fit the screen. Otherwise,
     * it is made to fit the screen according to the dimensions of the previous image matrix. This
     * allows the image to maintain its zoom after rotation.
     */
    private void FitImageToView() {
    	Drawable drawable = Drawable;
        if (drawable == null || drawable.IntrinsicWidth == 0 || drawable.IntrinsicHeight == 0) {
        	return;
        }
        if (matrix == null || prevMatrix == null) {
        	return;
        }
        
        int drawableWidth = drawable.IntrinsicWidth;
        int drawableHeight = drawable.IntrinsicHeight;
    	
    	//
    	// Scale image for view
    	//
        float scaleX = (float) viewWidth / drawableWidth;
        float scaleY = (float) viewHeight / drawableHeight;

        if (mScaleType == ImageView.ScaleType.Center)
        {
            scaleX = scaleY = 1;
        }
        else if (mScaleType == ImageView.ScaleType.CenterCrop)
        {
            scaleX = scaleY = System.Math.Max(scaleX, scaleY);
        }
        else if (mScaleType == ImageView.ScaleType.CenterInside)
        {
            scaleX = scaleY = System.Math.Min(1, System.Math.Min(scaleX, scaleY));
        }
        else if (mScaleType == ImageView.ScaleType.FitCenter)
        {
            scaleX = scaleY = System.Math.Min(scaleX, scaleY);
        } else if(mScaleType == ImageView.ScaleType.FitXy){
        }
        else
        {
            //
            // FIT_START and FIT_END not supported
            //
            throw new UnsupportedOperationException("TouchImageView does not support FIT_START or FIT_END");
        	
        }

        //
        // Center the image
        //
        float redundantXSpace = viewWidth - (scaleX * drawableWidth);
        float redundantYSpace = viewHeight - (scaleY * drawableHeight);
        matchViewWidth = viewWidth - redundantXSpace;
        matchViewHeight = viewHeight - redundantYSpace;
        if (!IsZoomed() && !imageRenderedAtLeastOnce) {
        	//
        	// Stretch and center image to fit view
        	//
        	matrix.SetScale(scaleX, scaleY);
        	matrix.PostTranslate(redundantXSpace / 2, redundantYSpace / 2);
        	normalizedScale = 1;
        	
        } else {
        	//
        	// These values should never be 0 or we will set viewWidth and viewHeight
        	// to NaN in translateMatrixAfterRotate. To avoid this, call savePreviousImageValues
        	// to set them equal to the current values.
        	//
        	if (prevMatchViewWidth == 0 || prevMatchViewHeight == 0) {
        		SavePreviousImageValues();
        	}
        	
        	prevMatrix.GetValues(m);
        	
        	//
        	// Rescale Matrix after rotation
        	//
        	m[Matrix.MscaleX] = matchViewWidth / drawableWidth * normalizedScale;
        	m[Matrix.MscaleY] = matchViewHeight / drawableHeight * normalizedScale;
        	
        	//
        	// TransX and TransY from previous matrix
        	//
            float transX = m[Matrix.MtransX];
            float transY = m[Matrix.MtransY];
            
            //
            // Width
            //
            float prevActualWidth = prevMatchViewWidth * normalizedScale;
            float actualWidth = GetImageWidth();
            TranslateMatrixAfterRotate(Matrix.MtransX, transX, prevActualWidth, actualWidth, prevViewWidth, viewWidth, drawableWidth);
            
            //
            // Height
            //
            float prevActualHeight = prevMatchViewHeight * normalizedScale;
            float actualHeight = GetImageHeight();
            TranslateMatrixAfterRotate(Matrix.MtransY, transY, prevActualHeight, actualHeight, prevViewHeight, viewHeight, drawableHeight);
            
            //
            // Set the matrix to the adjusted scale and translate values.
            //
            matrix.SetValues(m);
        }
        FixTrans();
        ImageMatrix = matrix;
    }
    
    /**
     * Set view dimensions based on layout params
     * 
     * @param mode 
     * @param size
     * @param drawableWidth
     * @return
     */
    private int SetViewSize(MeasureSpecMode mode, int size, int drawableWidth) {
    	int viewSize;
    	switch (mode) {
		case MeasureSpecMode.Exactly:
			viewSize = size;
			break;
			
		case MeasureSpecMode.AtMost:
			viewSize = System.Math.Min(drawableWidth, size);
			break;
			
		case MeasureSpecMode.Unspecified:
			viewSize = drawableWidth;
			break;
			
		default:
			viewSize = size;
		 	break;
		}
    	return viewSize;
    }
    
    /**
     * After rotating, the matrix needs to be translated. This function finds the area of image 
     * which was previously centered and adjusts translations so that is again the center, post-rotation.
     * 
     * @param axis Matrix.MTRANS_X or Matrix.MTRANS_Y
     * @param trans the value of trans in that axis before the rotation
     * @param prevImageSize the width/height of the image before the rotation
     * @param imageSize width/height of the image after rotation
     * @param prevViewSize width/height of view before rotation
     * @param viewSize width/height of view after rotation
     * @param drawableSize width/height of drawable
     */
    private void TranslateMatrixAfterRotate(int axis, float trans, float prevImageSize, float imageSize, int prevViewSize, int viewSize, int drawableSize) {
    	if (imageSize < viewSize) {
        	//
        	// The width/height of image is less than the view's width/height. Center it.
        	//
        	m[axis] = (viewSize - (drawableSize * m[Matrix.MscaleX])) * 0.5f;
        	
        } else if (trans > 0) {
        	//
        	// The image is larger than the view, but was not before rotation. Center it.
        	//
        	m[axis] = -((imageSize - viewSize) * 0.5f);
        	
        } else {
        	//
        	// Find the area of the image which was previously centered in the view. Determine its distance
        	// from the left/top side of the view as a fraction of the entire image's width/height. Use that percentage
        	// to calculate the trans in the new view width/height.
        	//
        	float percentage = (System.Math.Abs(trans) + (0.5f * prevViewSize)) / prevImageSize;
        	m[axis] = -((percentage * imageSize) - (viewSize * 0.5f));
        }
    }
    
    private void SetState(State state) {
    	this.state = state;
    }
    
    public bool CanScrollHorizontallyFroyo(int direction) {
        return CanScrollHorizontally(direction);
    }
    
    public bool CanScrollHorizontally(int direction) {
    	matrix.GetValues(m);
    	float x = m[Matrix.MtransX];
    	
    	if (GetImageWidth() < viewWidth) {
    		return false;
    		
    	} else if (x >= -1 && direction < 0) {
    		return false;
    		
    	} else if (System.Math.Abs(x) + viewWidth + 1 >= GetImageWidth() && direction > 0) {
    		return false;
    	}
    	
    	return true;
    }
    
    /**
     * Gesture Listener detects a single click or long click and passes that on
     * to the view's listener.
     * @author Ortiz
     *
     */
    private class GestureListener : GestureDetector.SimpleOnGestureListener {

        TouchImageView i;
        public GestureListener(TouchImageView imgView) {
            this.i = imgView;
        }

        public override bool OnSingleTapConfirmed(MotionEvent e)
        {
            if(i.doubleTapListener != null) {
            	return i.doubleTapListener.OnSingleTapConfirmed(e);
            }
        	return i.PerformClick();
        }

        public override void OnLongPress(MotionEvent e)
        {
        	i.PerformLongClick();
        }
        
        public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
        	if (i.fling != null) {
        		//
        		// If a previous fling is still active, it should be cancelled so that two flings
        		// are not run simultaenously.
        		//
        		i.fling.CancelFling();
        	}
        	i.fling = new Fling(i, (int) velocityX, (int) velocityY);
        	i.CompatPostOnAnimation(i.fling);
        	return base.OnFling(e1, e2, velocityX, velocityY);
        }

        public override bool OnDoubleTap(MotionEvent e)
        {
        	bool consumed = false;
            if(i.doubleTapListener != null) {
            	consumed = i.doubleTapListener.OnDoubleTap(e);
            }
        	if (i.state == State.NONE) {
	        	float targetZoom = (i.normalizedScale == i.minScale) ? i.maxScale : i.minScale;
	        	DoubleTapZoom doubleTap = new DoubleTapZoom(i, targetZoom, e.GetX(), e.GetY(), false);
	        	i.CompatPostOnAnimation(doubleTap);
	        	consumed = true;
        	}
        	return consumed;
        }

        public override bool OnDoubleTapEvent(MotionEvent e)
        {
            if(i.doubleTapListener != null) {
            	return i.doubleTapListener.OnDoubleTapEvent(e);
            }
            return false;
        }
    }
    
    public interface OnTouchImageViewListener {
    	void OnMove();
    }
    
    /**
     * Responsible for all touch events. Handles the heavy lifting of drag and also sends
     * touch events to Scale Detector and Gesture Detector.
     * @author Ortiz
     *
     */
    private class PrivateOnTouchListener : Java.Lang.Object, IOnTouchListener
    {

        TouchImageView i;
        public PrivateOnTouchListener(TouchImageView imgView) {
            this.i = imgView;
        }
    	
    	//
        // Remember last point position for dragging
        //
        private PointF last = new PointF();
    	
        public bool OnTouch(View v, MotionEvent ev) {
            i.mScaleDetector.OnTouchEvent(ev);
            i.mGestureDetector.OnTouchEvent(ev);
            PointF curr = new PointF(ev.GetX(), ev.GetY());
            
            if (i.state == State.NONE || i.state == State.DRAG || i.state == State.FLING) {
	            switch (ev.Action) {
	                case MotionEventActions.Down:
	                	last.Set(curr);
                        if (i.fling != null)
                            i.fling.CancelFling();
                        i.SetState(State.DRAG);
	                    break;

                    case MotionEventActions.Move:
                        if (i.state == State.DRAG)
                        {
	                        float deltaX = curr.X - last.X;
	                        float deltaY = curr.Y - last.Y;
                            float fixTransX = i.GetFixDragTrans(deltaX, i.viewWidth, i.GetImageWidth());
                            float fixTransY = i.GetFixDragTrans(deltaY, i.viewHeight, i.GetImageHeight());
                            i.matrix.PostTranslate(fixTransX, fixTransY);
                            i.FixTrans();
	                        last.Set(curr.X, curr.Y);
	                    }
	                    break;

                    case MotionEventActions.Up:
                    case MotionEventActions.PointerUp:
                        i.SetState(State.NONE);
	                    break;
	            }
            }
            
            i.ImageMatrix = i.matrix;
            
            //
    		// User-defined OnTouchListener
    		//
    		if(i.userTouchListener != null) {
    			i.userTouchListener.OnTouch(v, ev);
    		}
            
    		//
    		// OnTouchImageViewListener is set: TouchImageView dragged by user.
    		//
    		if (i.touchImageViewListener != null) {
    			i.touchImageViewListener.OnMove();
    		}
    		
            //
            // indicate event was handled
            //
            return true;
        }
    }

    /**
     * ScaleListener detects user two finger scaling and scales image.
     * @author Ortiz
     *
     */
    private class ScaleListener : ScaleGestureDetector.SimpleOnScaleGestureListener {

        TouchImageView i;
        public ScaleListener(TouchImageView imgView)
        {
            this.i = imgView;
        }
        
        public override bool OnScaleBegin(ScaleGestureDetector detector) {
            i.SetState(State.ZOOM);
            return true;
        }

        public override bool OnScale(ScaleGestureDetector detector)
        {
            i.ScaleImage(detector.ScaleFactor, detector.FocusX, detector.FocusY, true);
        	
        	//
        	// OnTouchImageViewListener is set: TouchImageView pinch zoomed by user.
        	//
            if (i.touchImageViewListener != null)
            {
                i.touchImageViewListener.OnMove();
        	}
            return true;
        }

        public override void OnScaleEnd(ScaleGestureDetector detector)
        {
            //base.OnScaleEnd(detector)
            i.SetState(State.NONE);
        	bool animateToZoomBoundary = false;
            float targetZoom = i.normalizedScale;
            if (i.normalizedScale > i.maxScale)
            {
                targetZoom = i.maxScale;
        		animateToZoomBoundary = true;

            }
            else if (i.normalizedScale < i.minScale)
            {
                targetZoom = i.minScale;
        		animateToZoomBoundary = true;
        	}
        	
        	if (animateToZoomBoundary) {
                DoubleTapZoom doubleTap = new DoubleTapZoom(i, targetZoom, i.viewWidth / 2, i.viewHeight / 2, true);
                i.CompatPostOnAnimation(doubleTap);
        	}
        }
    }
    
    private void ScaleImage(double deltaScale, float focusX, float focusY, bool stretchImageToSuper) {
    	
    	float lowerScale, upperScale;
    	if (stretchImageToSuper) {
    		lowerScale = superMinScale;
    		upperScale = superMaxScale;
    		
    	} else {
    		lowerScale = minScale;
    		upperScale = maxScale;
    	}
    	
    	float origScale = normalizedScale;
        normalizedScale *= (float)deltaScale;
        if (normalizedScale > upperScale) {
            normalizedScale = upperScale;
            deltaScale = upperScale / origScale;
        } else if (normalizedScale < lowerScale) {
            normalizedScale = lowerScale;
            deltaScale = lowerScale / origScale;
        }
        
        matrix.PostScale((float) deltaScale, (float) deltaScale, focusX, focusY);
        FixScaleTrans();
    }
    
    /**
     * DoubleTapZoom calls a series of runnables which apply
     * an animated zoom in/out graphic to the image.
     * @author Ortiz
     *
     */
    private class DoubleTapZoom : Java.Lang.Object, IRunnable {
    	
    	private long startTime;
    	private static readonly float ZOOM_TIME = 500;
    	private float startZoom, targetZoom;
    	private float bitmapX, bitmapY;
    	private bool stretchImageToSuper;
    	private AccelerateDecelerateInterpolator interpolator = new AccelerateDecelerateInterpolator();
    	private PointF startTouch;
    	private PointF endTouch;

        private TouchImageView i;

    	public DoubleTapZoom(TouchImageView imgView, float targetZoom, float focusX, float focusY, bool stretchImageToSuper) {
            this.i = imgView;
    		i.SetState(State.ANIMATE_ZOOM);
    		startTime = DateTime.Now.Ticks;
            this.startZoom = i.normalizedScale;
    		this.targetZoom = targetZoom;
    		this.stretchImageToSuper = stretchImageToSuper;
            PointF bitmapPoint = i.TransformCoordTouchToBitmap(focusX, focusY, false);
    		this.bitmapX = bitmapPoint.X;
    		this.bitmapY = bitmapPoint.Y;
    		
    		//
    		// Used for translating image during scaling
    		//
            startTouch = i.TransformCoordBitmapToTouch(bitmapX, bitmapY);
            endTouch = new PointF(i.viewWidth / 2, i.viewHeight / 2);
    	}

		public void Run() {
			float t = Interpolate();
			double deltaScale = CalculateDeltaScale(t);
            i.ScaleImage(deltaScale, bitmapX, bitmapY, stretchImageToSuper);
			TranslateImageToCenterTouchPosition(t);
            i.FixScaleTrans();
            i.ImageMatrix = i.matrix;
			
			//
			// OnTouchImageViewListener is set: double tap runnable updates listener
			// with every frame.
			//
            if (i.touchImageViewListener != null)
            {
                i.touchImageViewListener.OnMove();
			}
			
			if (t < 1f) {
				//
				// We haven't finished zooming
				//
				i.CompatPostOnAnimation(this);
				
			} else {
				//
				// Finished zooming
				//
                i.SetState(State.NONE);
			}
		}
		
		/**
		 * Interpolate between where the image should start and end in order to translate
		 * the image so that the point that is touched is what ends up centered at the end
		 * of the zoom.
		 * @param t
		 */
		private void TranslateImageToCenterTouchPosition(float t) {
			float targetX = startTouch.X + t * (endTouch.X - startTouch.X);
			float targetY = startTouch.Y + t * (endTouch.Y - startTouch.Y);
            PointF curr = i.TransformCoordBitmapToTouch(bitmapX, bitmapY);
            i.matrix.PostTranslate(targetX - curr.X, targetY - curr.Y);
		}
		
		/**
		 * Use interpolator to get t
		 * @return
		 */
		private float Interpolate() {
			long currTime = DateTime.Now.Ticks;
			float elapsed = (currTime - startTime) / ZOOM_TIME;
			elapsed = System.Math.Min(1f, elapsed);
			return interpolator.GetInterpolation(elapsed);
		}
		
		/**
		 * Interpolate the current targeted zoom and get the delta
		 * from the current zoom.
		 * @param t
		 * @return
		 */
		private double CalculateDeltaScale(float t) {
			double zoom = startZoom + t * (targetZoom - startZoom);
            return zoom / i.normalizedScale;
		}
    }
    
    /**
     * This function will transform the coordinates in the touch event to the coordinate 
     * system of the drawable that the imageview contain
     * @param x x-coordinate of touch event
     * @param y y-coordinate of touch event
     * @param clipToBitmap Touch event may occur within view, but outside image content. True, to clip return value
     * 			to the bounds of the bitmap size.
     * @return Coordinates of the point touched, in the coordinate system of the original drawable.
     */
    private PointF TransformCoordTouchToBitmap(float x, float y, bool clipToBitmap) {
         matrix.GetValues(m);
         float origW = Drawable.IntrinsicWidth;
         float origH = Drawable.IntrinsicHeight;
         float transX = m[Matrix.MtransX];
         float transY = m[Matrix.MtransY];
         float finalX = ((x - transX) * origW) / GetImageWidth();
         float finalY = ((y - transY) * origH) / GetImageHeight();
         
         if (clipToBitmap) {
             finalX = System.Math.Min(System.Math.Max(finalX, 0), origW);
             finalY = System.Math.Min(System.Math.Max(finalY, 0), origH);
         }
         
         return new PointF(finalX , finalY);
    }
    
    /**
     * Inverse of transformCoordTouchToBitmap. This function will transform the coordinates in the
     * drawable's coordinate system to the view's coordinate system.
     * @param bx x-coordinate in original bitmap coordinate system
     * @param by y-coordinate in original bitmap coordinate system
     * @return Coordinates of the point in the view's coordinate system.
     */
    private PointF TransformCoordBitmapToTouch(float bx, float by) {
        matrix.GetValues(m);        
        float origW = Drawable.IntrinsicWidth;
        float origH = Drawable.IntrinsicHeight;
        float px = bx / origW;
        float py = by / origH;
        float finalX = m[Matrix.MtransX] + GetImageWidth() * px;
        float finalY = m[Matrix.MtransY] + GetImageHeight() * py;
        return new PointF(finalX , finalY);
    }
    
    /**
     * Fling launches sequential runnables which apply
     * the fling graphic to the image. The values for the translation
     * are interpolated by the Scroller.
     * @author Ortiz
     *
     */
    private class Fling : Java.Lang.Object, IRunnable
    {
    	
        CompatScroller scroller;
    	int currX, currY;
    	
        TouchImageView i;

    	public Fling(TouchImageView imgView, int velocityX, int velocityY) {
            this.i = imgView;
    		i.SetState(State.FLING);
            scroller = new CompatScroller(i.context);
            i.matrix.GetValues(i.m);

            int startX = (int)i.m[Matrix.MtransX];
            int startY = (int)i.m[Matrix.MtransY];
    		int minX, maxX, minY, maxY;

            if (i.GetImageWidth() > i.viewWidth)
            {
                minX = i.viewWidth - (int)i.GetImageWidth();
    			maxX = 0;
    			
    		} else {
    			minX = maxX = startX;
    		}

            if (i.GetImageHeight() > i.viewHeight)
            {
                minY = i.viewHeight - (int)i.GetImageHeight();
    			maxY = 0;
    			
    		} else {
    			minY = maxY = startY;
    		}
    		
    		scroller.Fling(startX, startY, (int) velocityX, (int) velocityY, minX,
                    maxX, minY, maxY);
    		currX = startX;
    		currY = startY;
    	}
    	
    	public void CancelFling() {
    		if (scroller != null) {
                i.SetState(State.NONE);
    			scroller.ForceFinished(true);
    		}
    	}
    	
		public void Run() {
			
			//
			// OnTouchImageViewListener is set: TouchImageView listener has been flung by user.
			// Listener runnable updated with each frame of fling animation.
			//
            if (i.touchImageViewListener != null)
            {
                i.touchImageViewListener.OnMove();
			}
			
			if (scroller.IsFinished()) {
        		scroller = null;
        		return;
        	}
			
			if (scroller.ComputeScrollOffset()) {
	        	int newX = scroller.GetCurrX();
	            int newY = scroller.GetCurrY();
	            int transX = newX - currX;
	            int transY = newY - currY;
	            currX = newX;
	            currY = newY;
                i.matrix.PostTranslate(transX, transY);
                i.FixTrans();
                i.ImageMatrix = i.matrix;
                i.CompatPostOnAnimation(this);
        	}
		}
    }
    
	private class CompatScroller {
    	Scroller scroller;
    	OverScroller overScroller;
    	bool isPreGingerbread;
    	
    	public CompatScroller(Context context) {

            if (Build.VERSION.SdkInt < BuildVersionCodes.Gingerbread)
            {
                isPreGingerbread = true;
                scroller = new Scroller(context);
    			
            } else {
    			isPreGingerbread = false;
    			overScroller = new OverScroller(context);
            }
    	}
    	
    	public void Fling(int startX, int startY, int velocityX, int velocityY, int minX, int maxX, int minY, int maxY) {
    		if (isPreGingerbread) {
    			scroller.Fling(startX, startY, velocityX, velocityY, minX, maxX, minY, maxY);
    		} else {
    			overScroller.Fling(startX, startY, velocityX, velocityY, minX, maxX, minY, maxY);
    		}
    	}
    	
    	public void ForceFinished(bool finished) {
    		if (isPreGingerbread) {
    			scroller.ForceFinished(finished);
    		} else {
    			overScroller.ForceFinished(finished);
    		}
    	}
    	
    	public bool IsFinished() {
    		if (isPreGingerbread) {
    			return scroller.IsFinished;
    		} else {
    			return overScroller.IsFinished;
    		}
    	}
    	
    	public bool ComputeScrollOffset() {
    		if (isPreGingerbread) {
    			return scroller.ComputeScrollOffset();
    		} else {
    			overScroller.ComputeScrollOffset();
    			return overScroller.ComputeScrollOffset();
    		}
    	}
    	
    	public int GetCurrX() {
    		if (isPreGingerbread) {
    			return scroller.CurrX;
    		} else {
    			return overScroller.CurrX;
    		}
    	}
    	
    	public int GetCurrY() {
    		if (isPreGingerbread) {
    			return scroller.CurrY;
    		} else {
    			return overScroller.CurrY;
    		}
    	}
    }
    
	private void CompatPostOnAnimation(IRunnable runnable) {
        //if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwichMr1)//JELLY_BEAN
        //{
        //    PostOnAnimation(runnable);
            
        //} else {
            PostDelayed(runnable, 1000/60);
        //}
    }
    
    private class ZoomVariables {
    	public float scale;
    	public float focusX;
    	public float focusY;
    	public ScaleType scaleType;
    	
    	public ZoomVariables(float scale, float focusX, float focusY, ScaleType scaleType) {
    		this.scale = scale;
    		this.focusX = focusX;
    		this.focusY = focusY;
    		this.scaleType = scaleType;
    	}
    }
    
    private void PrintMatrixInfo() {
    	float[] n = new float[9];
    	matrix.GetValues(n);
    	//Log.d(DEBUG, "Scale: " + n[Matrix.MSCALE_X] + " TransX: " + n[Matrix.MTRANS_X] + " TransY: " + n[Matrix.MTRANS_Y]);
    }
    }
}