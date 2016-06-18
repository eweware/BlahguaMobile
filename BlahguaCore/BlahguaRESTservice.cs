using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Text;
using RestSharp;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using ServiceStack.Text;
using System.Security.Cryptography;

namespace BlahguaMobile.BlahguaCore
{
	public delegate void ChannelList_callback(ChannelList theList);
	public delegate void ChannelTypeList_callback(ChannelTypeList theList);
	public delegate void Inbox_callback(Inbox theList);
	public delegate void BlahTypes_callback(BlahTypeList theList);
	public delegate void Blah_callback(Blah theBlah);
	public delegate void Comment_callback(Comment theBlah);
	public delegate void UserDescription_callback(UserDescription theDesc);
	public delegate void UserDescriptionList_callback(UserDescriptionList theDescList);
	public delegate void Comments_callback(CommentList theList);
	public delegate void Blahs_callback(BlahList theList);
	public delegate void string_callback(String theResult);
	public delegate void User_callback(User theResult);
	public delegate void BadgeAuthorities_callback(BadgeAuthorityList theResult);
	public delegate void int_callback(int theResult);
	public delegate void BadgeRecord_callback(BadgeRecord theResult);
	public delegate void PredictionVote_callback(UserPredictionVote theResult);
	public delegate void Stats_callback(Stats theResult);
	public delegate void ProfileSchema_callback(ProfileSchema theResult);
	public delegate void Profile_callback(UserProfile theResult); 
	public delegate void bool_callback(bool theResult);
	public delegate void WhatsNew_callback(WhatsNewInfo theResult);
	public delegate void ChannelPermissions_callback(ChannelPermissions theResult);
    public delegate void InboxBlah_callback(InboxBlah theResult);


	public class BlahguaRESTservice
	{
		public Dictionary<string, string> groupNames = null;
		public Dictionary<string, string> userGroupNames = null;
		public Dictionary<string, string> blahTypes = null;
		public string BaseShareURL { get; set; }
		public static bool usingQA = true;//false; //false; //true;
		private RestClient apiClient;
		private string imageBaseURL = "";
		private static string apiPath = "/api/v3";
		private static string localHostPath = "http://localhost:8080";
		private static string networkHostPath = "http://192.168.0.6:8080";
		private static string prodHostPath = "https://heard-gae.appspot.com";
		private static string serverPath = networkHostPath;

		public BlahguaRESTservice()
		{
			System.Console.WriteLine("Using server @" + serverPath + apiPath);
			apiClient = new RestClient(serverPath + apiPath);
			BaseShareURL = serverPath;
			imageBaseURL = "https://s3-us-west-2.amazonaws.com/qa.blahguaimages/image/";


			apiClient.CookieContainer = new CookieContainer();
		}



		public string ImageBaseURL
		{
			get { return imageBaseURL; }
		}

        
		public void GetBlahComments(long blahId, Comments_callback callback)
		{
			RestRequest request = new RestRequest("comments", Method.GET);
			request.AddParameter("blahId", blahId);
			apiClient.ExecuteAsync<CommentList>(request, (response) =>
				{
					callback(response.Data);
				});
		}

		public void GetBlahTopComments(long blahId, Comments_callback callback)
		{
			RestRequest request = new RestRequest("comments/hot", Method.GET);
			request.AddParameter("blahId", blahId);
			apiClient.ExecuteAsync(request, (response) =>
				{
					CommentList commentList = null;
					try
					{
						string resStr = response.Content;
						resStr = resStr.Replace("\"D\":", "\"Downvotes\":");
						resStr = resStr.Replace("\"U\":", "\"Upvotes\":");
						commentList = resStr.FromJson<CommentList>();
					}
					catch (SerializationException)
					{
						commentList = null;
					}

					callback(commentList);
				});
		}

		public void GetWhatsNew(WhatsNew_callback theCallback)
		{
			RestRequest request = new RestRequest("users/whatsnew", Method.GET);
			apiClient.ExecuteAsync(request, (response) =>
				{
					WhatsNewInfo newInfo = response.Content.FromJson<WhatsNewInfo>();

					theCallback(newInfo);
				});
		}

		public void GetChannelPermissionById(long channelId, ChannelPermissions_callback theCallback)
		{
			RestRequest request = new RestRequest ("ChannelPermission", Method.GET);
			request.AddParameter ("id", channelId);
			apiClient.ExecuteAsync(request, (response) =>
				{
					ChannelPermissions thePerm = response.Content.FromJson<ChannelPermissions>();

					theCallback(thePerm);
				});
		}



        public void GetAdForUser(InboxBlah_callback callback)
        {
            RestRequest request = new RestRequest("users/ad", Method.GET);
            apiClient.ExecuteAsync(request, (response) => 
                {
                    InboxBlah theAd = null;
                    if (response.StatusCode == HttpStatusCode.OK)
                        theAd = response.Content.FromJson<InboxBlah>();
                    
                    callback(theAd);
                });

        }

		public void GetBadgeAuthorities(BadgeAuthorities_callback callback)
		{
			RestRequest request = new RestRequest("badges/authorities", Method.GET);
			apiClient.ExecuteAsync(request, (response) =>
				{
					BadgeAuthorityList theList = response.Content.FromJson<BadgeAuthorityList>();
					callback(theList);
				});
		}


		public void RecordImpressions(Dictionary<long, int> impressionMap, int_callback callback)
		{
			RestRequest request = new RestRequest("blahs/counts", Method.PUT);
			string jsonString = "";
			foreach (long curKey in impressionMap.Keys)
			{
				if (jsonString != "")
					jsonString += ", ";
				jsonString += "\"" + curKey + "\": " + impressionMap[curKey].ToString();
			}

			jsonString  = "{\"V\":{" + jsonString + "}}";
			request.AddParameter("application/json", jsonString, ParameterType.RequestBody); 
			apiClient.ExecuteAsync(request, (response) =>
				{
					if (callback != null)
						callback(impressionMap.Count);
				});
		}

		public void AddBlahOpen(long blahId)
		{
			try
			{
				RestRequest request = new RestRequest("blahs", Method.PUT);
				string jsonString = "{\"blahId\":" + blahId + ", \"O\":1, \"stats\": true}";
				request.AddParameter("application/json", jsonString, ParameterType.RequestBody);

				apiClient.ExecuteAsync(request, (response) =>
					{

					});
			}
			catch (Exception exp)
			{
				Console.WriteLine("err: " + exp.Message);
			}
		}

		public void ReportPost(long contentId, int reportType)
		{
			RestRequest request = new RestRequest("ReportContent", Method.POST);
			string jsonString = "{\"type\": " + reportType + ", \"blah\": true, \"id\":" + contentId + "}";

			request.AddParameter("application/json", jsonString, ParameterType.RequestBody);
			apiClient.ExecuteAsync(request, (response) =>
				{

				});
		}

		public void ReportComment(long contentId, int reportType)
		{
			RestRequest request = new RestRequest("ReportContent", Method.POST);
			string jsonString = "{\"type\": " + reportType + ", \"comment\": true, \"id\":" + contentId + "}";

			request.AddParameter("application/json", jsonString, ParameterType.RequestBody);
			apiClient.ExecuteAsync(request, (response) =>
				{

				});
		}

		public void ReportUser(long contentId, int reportType)
		{
			RestRequest request = new RestRequest("ReportContent", Method.POST);
			string jsonString = "{\"type\": " + reportType + ", \"user\": true, \"id\":" + contentId + "}";

			request.AddParameter("application/json", jsonString, ParameterType.RequestBody);
			apiClient.ExecuteAsync(request, (response) =>
				{

				});
		}

		public void CreateBadgeForUser(long authorityId, string badgeTypeId, string_callback callback)
		{
			RestRequest request = new RestRequest("badges", Method.POST);
			request.AddHeader("Accept", "*/*");
			request.RequestFormat = DataFormat.Json;
			request.AddBody(new { I = authorityId });

			apiClient.ExecuteAsync(request, (response) =>
				{
					callback(response.Content);
				});


		}



		public void GetBlahWithStats(long blahId, DateTime startDate, DateTime endDate, Blah_callback callback)
		{
			string startDateString = Utilities.CreateDateString(startDate, false);
			string endDateString = Utilities.CreateDateString(endDate, false);

			RestRequest request = new RestRequest("blahs/" + blahId, Method.GET);
			request.AddParameter("stats", "true");
			request.AddParameter("s", startDateString);
			request.AddParameter("e", endDateString);

			apiClient.ExecuteAsync(request, (response) =>
				{
					string resStr = response.Content;
					resStr = resStr.Replace("\"c\":", "\"cdate\":");
					Blah theBlah = resStr.FromJson<Blah>();


					callback(theBlah);
				});




		}

		public void GetUserComments(long userId, Comments_callback callback)
		{
			RestRequest request = new RestRequest("comments", Method.GET);
			request.AddParameter("authorId", userId);
			apiClient.ExecuteAsync(request, (response) =>
				{
					CommentList commentList = null;

					string resStr = response.Content;
					resStr = resStr.Replace("\"D\":", "\"Downvotes\":");
					resStr = resStr.Replace("\"U\":", "\"Upvotes\":");
					commentList = resStr.FromJson<CommentList>();

					callback(commentList);
				});
		}

		public void GetUserBlahs(Blahs_callback callback)
		{
			RestRequest request = new RestRequest("blahs", Method.GET);
			apiClient.ExecuteAsync(request, (response) =>
				{
					BlahList blahList = null;
					try
					{
						string resStr = response.Content;
						resStr = resStr.Replace("\"c\":", "\"cdate\":");
						BlahList altList = resStr.FromJson<BlahList>();


						if (altList != null)
							blahList = altList;
					}
					catch (SerializationException)
					{

					}

					callback(blahList);
				});
		}

		public void AddUserToChannel(long channelId, string_callback callback)
		{
			RestRequest request = new RestRequest("userGroups", Method.POST);
			request.RequestFormat = DataFormat.Json;
			request.AddBody(new { G = channelId });

			apiClient.ExecuteAsync(request, (response) =>
				{
					callback(response.Content);
				});
		}

		public void GetPublicChannels(bool bIncludeHidden, ChannelList_callback callback)
		{
			RestRequest request = new RestRequest("groups/featured", Method.GET);
			apiClient.ExecuteAsync(request, (response) =>
				{
					ChannelList theList = response.Content.FromJson<ChannelList>();

					if (theList != null)
					{
						ChannelList newList = new ChannelList();

						foreach (Channel curChan in theList)
						{
							if (bIncludeHidden || (curChan.R > 0))
								newList.Add(curChan);
						}


						newList.Sort((obj1, obj2) =>
							{
								return obj1.R.CompareTo(obj2.R);
							});

						callback(newList);
					}
					else
						callback(null);
				});

		}

		public void GetProfileSchema(ProfileSchema_callback callback)
		{
			RestRequest request = new RestRequest("users/profile/schema", Method.GET);
			apiClient.ExecuteAsync<ProfileSchema>(request, (response) =>
				{
					callback(response.Data);    
				});

		}

		public void GetUserProfile(Profile_callback callback)
		{
			RestRequest request = new RestRequest("users/profile/info", Method.GET);
			apiClient.ExecuteAsync(request, (response) =>
				{
					string resStr = response.Content;
					resStr = resStr.Replace("\"c\":", "\"cdate\":");
					UserProfile profile = resStr.FromJson<UserProfile>();

					callback(profile);
				});

		}

		public void UpdateUserName(string userName, string_callback callback)
		{
			RestRequest request = new RestRequest("users/profile/info", Method.PUT);

			request.RequestFormat = DataFormat.Json;
			request.AddBody(new { A = userName});

			apiClient.ExecuteAsync(request, (response) =>
				{
					callback("ok");
				});

		}

		public void DeleteUserImage(string_callback callback)
		{
			RestRequest request = new RestRequest("users/images", Method.DELETE);


			apiClient.ExecuteAsync(request, (response) =>
				{
					callback("ok");
				});

		}

		public void DeleteBlah(long blahId, string_callback callback)
		{
			RestRequest request = new RestRequest("blahs/" + blahId, Method.DELETE);


			apiClient.ExecuteAsync(request, (response) =>
				{
					callback("ok");
				});

		}



		public void UpdateUserProfile(UserProfile theProfile, string_callback callback)
		{
			RestRequest request = new RestRequest("users/profile/info", Method.PUT);
			MemoryStream ms = new MemoryStream();

			// Serializer the User object to the stream.
			DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(UserProfile));
			ser.WriteObject(ms, theProfile);
			byte[] json = ms.ToArray();
			ms.Close();
			string dataStr = Encoding.UTF8.GetString(json, 0, json.Length);
			request.AddParameter("application/json", dataStr, ParameterType.RequestBody); 

			apiClient.ExecuteAsync(request, (response) =>
				{
					callback("ok");
				});

		}

		public void GetUserChannels(ChannelList_callback callback)
		{
			RestRequest request = new RestRequest("userGroups", Method.GET);
			apiClient.ExecuteAsync<ChannelList>(request, (response) =>
				{
					if (response.Data != null)
					{
						ChannelList newList = response.Data;
						newList.Sort((obj1, obj2) =>
							{
								return Math.Abs(obj1.R).CompareTo(Math.Abs(obj2.R));
							});

						callback(newList);
					} 
					else
					{
						string rawJSON = response.Content;
						ChannelList newList = rawJSON.FromJson<ChannelList>();
						newList.Sort((obj1, obj2) =>
							{
								return Math.Abs(obj1.R).CompareTo(Math.Abs(obj2.R));
							});
						callback(newList);
					}
				});

		}

		public void SignIn(string userName, string passWord, string_callback callback)
		{
			RestRequest request = new RestRequest("users/login", Method.POST);
			request.RequestFormat = DataFormat.Json;
			request.AddBody(new { N = userName, pwd = passWord });

			apiClient.ExecuteAsync(request, (response) =>
				{
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        VerifyClient((didIt) =>
                        {
                            callback("");
                        });
                    }
                    else
                    {
                        string resultStr = "Error";

                        if (!String.IsNullOrEmpty(response.StatusDescription))
                            resultStr = response.StatusDescription;
                        else if (!String.IsNullOrEmpty(response.ErrorMessage))
                            resultStr = response.ErrorMessage;
                        callback(resultStr);
                    }
				});

		}

        public void VerifyClient(bool_callback callback)
        {
            RestRequest request = new RestRequest("users/client", Method.GET);
            apiClient.ExecuteAsync(request, (response) =>
	            {
	                bool didIt = false;

	                string keyString = response.Content;

	                string encryptStr = encrypt(keyString);
	                RestRequest authRequest = new RestRequest("users/client", Method.POST);
	                authRequest.RequestFormat = DataFormat.Json;
	                authRequest.AddBody(new { code = encryptStr });
	                apiClient.ExecuteAsync(authRequest, (authResp) =>
		                {
							string bool1str = "true";
							string bool2str = "True";
							bool bool1 = bool1str.FromJson<bool>();
							bool bool2 = bool2str.FromJsv<bool>();
							didIt = authResp.Content.FromJson<bool>();
		                    callback(didIt);
		                });
	            });
        }

        private static string  SECRET_CLIENT_KEY = "ImAGZFi9J9NfcKjPNljZfw==";

        private string encrypt(string theMessage)
        {
            string resultMsg = "";

            try
            {
                using (AesManaged myAES = new AesManaged())
                {
                    byte[] KeyArrBytes32Value = System.Convert.FromBase64String(SECRET_CLIENT_KEY);

                    myAES.BlockSize = 128;
                    myAES.KeySize = 128;

                    // It is equal in java 
                    /// Cipher _Cipher = Cipher.getInstance("AES/CBC/PKCS5PADDING");    
                    myAES.Mode = CipherMode.CBC;
                    myAES.Padding = PaddingMode.PKCS7;



                    // Initialization vector.   
                    // It could be any value or generated using a random number generator.
                    byte[] ivArr = { 1, 2, 3, 4, 5, 6, 6, 5, 4, 3, 2, 1, 7, 7, 7, 7 };
                    byte[] IVBytes16Value = new byte[16];
                    Array.Copy(ivArr, IVBytes16Value, 16);

                    myAES.Key = KeyArrBytes32Value;
                    myAES.IV = IVBytes16Value;

                    ICryptoTransform encrypto = myAES.CreateEncryptor();

                    byte[] plainTextByte = ASCIIEncoding.UTF8.GetBytes(theMessage);
                    byte[] CipherText = encrypto.TransformFinalBlock(plainTextByte, 0, plainTextByte.Length);
                    string endStr = Convert.ToBase64String(CipherText);
                    resultMsg = endStr;

                   
                }
            }
            catch (Exception exp)
            {
                System.Console.WriteLine(exp.Message);
            }

            return resultMsg;
        }

		public void CheckUserSignIn(bool_callback callback)
		{
			RestRequest request = new RestRequest("users/login/check", Method.GET);

			apiClient.ExecuteAsync<SigninStatus>(request, (response) =>
				{
					if ((response.Data != null) && (response.Data.loggedIn == "Y"))
						callback(true);
					else
						callback(false);
				});

		}

		public void SignOut(string_callback callback)
		{
			RestRequest request = new RestRequest("users/logout", Method.POST);
			request.RequestFormat = DataFormat.Json;
			apiClient.ExecuteAsync(request, (response) =>
				{
					callback(response.Content);
				});

		}

		public void Register(string userName, string passWord, string_callback callback)
		{
			RestRequest request = new RestRequest("users", Method.POST);
			request.RequestFormat = DataFormat.Json;
			request.AddBody(new { N = userName, pwd = passWord });
			apiClient.ExecuteAsync<User>(request, (response) =>
				{
					if (response.StatusCode != HttpStatusCode.Created)
					{
						string resultStr = "Error";

						if (!String.IsNullOrEmpty(response.StatusDescription))
							resultStr = response.StatusDescription;
						else if (!String.IsNullOrEmpty(response.ErrorMessage))
							resultStr = response.ErrorMessage;
						callback(resultStr);
					}
					else
						callback("");
				});

		}

		public void CreateBlah(BlahCreateRecord theBlah , Blah_callback callback)
		{
			RestRequest request = new RestRequest("blahs/new", Method.POST);
			request.RequestFormat = DataFormat.Json;
			JsConfig.DateHandler = JsonDateHandler.ISO8601;
			string theStr = theBlah.ToJson();
			request.AddParameter("application/json", theStr, ParameterType.RequestBody);
			apiClient.ExecuteAsync(request, (response) =>
				{
					Blah newBlah = null;
					if (response.StatusCode == HttpStatusCode.OK)
					{
						newBlah = response.Content.FromJson<Blah>();
						newBlah.cdate = DateTime.Now;

					}

					callback(newBlah);
				});
		}

		public void CreateComment(CommentCreateRecord theComment, Comment_callback callback)
		{
			RestRequest request = new RestRequest("comments", Method.POST);
			request.RequestFormat = DataFormat.Json;
			request.AddBody(theComment);
			apiClient.ExecuteAsync<Comment>(request, (response) =>
				{
					callback(response.Data);
				});
		}

        public void GetComment(long commentId, Comment_callback callback)
        {
            RestRequest request = new RestRequest("comments/" + commentId, Method.GET);
            request.RequestFormat = DataFormat.Json;
            apiClient.ExecuteAsync<Comment>(request, (response) =>
            {
                callback(response.Data);
            });
        }

		public void SetBlahVote(long blahId, int userVote, int_callback callback)
		{
			RestRequest request = new RestRequest("blahs", Method.PUT);
			request.RequestFormat = DataFormat.Json;
			request.AddBody(new { vote = userVote, id = blahId });
			apiClient.ExecuteAsync<int>(request, (response) =>
				{
					callback(userVote);
				});
		}

		public void SetCommentVote(long commentId, int userVote, int_callback callback)
		{
			RestRequest request = new RestRequest("comments", Method.PUT);
			request.AddParameter("id", commentId);
			request.AddParameter("vote", userVote);
			request.RequestFormat = DataFormat.Json;
			apiClient.ExecuteAsync<int>(request, (response) =>
				{
					callback(userVote);
				});
		}


		public void UploadPhoto(Stream photoStream, string fileName, string_callback callback)
		{
			string uploadURL = GetImageUploadURL ();
            int pathSplit =  uploadURL.IndexOf("/", 7);
            string appPath = uploadURL.Substring(0, pathSplit);
            string requestPath = uploadURL.Substring(pathSplit);
            RestClient onetimeClient = new RestClient(appPath);
			var request = new RestRequest(requestPath, Method.POST);
			request.AddHeader("Accept", "*/*"); 
			request.AddParameter("objectType", "X");
			request.AddParameter("objectId", "");
			request.AddParameter("primary", "true");
			request.AddFile("file", ReadToEnd(photoStream), fileName, "image/jpeg");

			onetimeClient.ExecuteAsync(request, (response) =>
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						callback(response.Content);
					}
					else
					{
						//error ocured during upload
						callback(null);
					}
				});
		}

		public string GetImageUploadURL()
		{
			RestRequest request = new RestRequest("uploadimage", Method.GET);
			IRestResponse response = apiClient.Execute (request);
			return response.Content;
		}


		public void UploadObjectPhoto(long objectId, string objectType, Stream photoStream, string fileName, string_callback callback)
		{
			string uploadURL = GetImageUploadURL ();
			var request = new RestRequest("images/upload", Method.POST);
			request.AddHeader("Accept", "*/*");
			//request.AlwaysMultipartFormData = true;
			request.AddParameter("objectType", objectType);
			request.AddParameter("objectId", objectId);
			request.AddParameter("primary", "true");
			request.AddFile("file", ReadToEnd(photoStream), fileName, "image/jpeg");

			apiClient.ExecuteAsync(request, (response) =>
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						callback(response.Content);
					}
					else
					{
						//error ocured during upload
						callback(null);
					}
				});
		}

		//method for converting stream to byte[]
		public byte[] ReadToEnd(System.IO.Stream stream)
		{
			long originalPosition = stream.Position;
			stream.Position = 0;

			try
			{
				byte[] readBuffer = new byte[4096];

				int totalBytesRead = 0;
				int bytesRead;

				while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
				{
					totalBytesRead += bytesRead;

					if (totalBytesRead == readBuffer.Length)
					{
						int nextByte = stream.ReadByte();
						if (nextByte != -1)
						{
							byte[] temp = new byte[readBuffer.Length * 2];
							Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
							Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
							readBuffer = temp;
							totalBytesRead++;
						}
					}
				}

				byte[] buffer = readBuffer;
				if (readBuffer.Length != totalBytesRead)
				{
					buffer = new byte[totalBytesRead];
					Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
				}
				return buffer;
			}
			finally
			{
				stream.Position = originalPosition;
			}
		}


		public void GetUserInfo(User_callback callback)
		{
			RestRequest request = new RestRequest("users/info", Method.GET);
			apiClient.ExecuteAsync<User>(request, (response) =>
				{
					callback(response.Data);
				});
		}

		public void GetRecoveryEmail(string_callback callback)
		{
			RestRequest request = new RestRequest("users/account", Method.GET);
			apiClient.ExecuteAsync<RecoveryInfo>(request, (response) =>
				{
					if (response.Data != null)
						callback(response.Data.E);
					else
						callback(null);
				});
		}

		public void SetRecoveryEmail(string newMail, string_callback callback)
		{
			RestRequest request = new RestRequest("users/account", Method.POST);
			request.RequestFormat = DataFormat.Json;
			request.AddBody(new { E = newMail });

			apiClient.ExecuteAsync(request, (response) =>
				{
					callback(newMail);
				});
		}

		public void UpdatePassword(string newPassword, string_callback callback)
		{
			RestRequest request = new RestRequest("users/update/password", Method.PUT);
			request.RequestFormat = DataFormat.Json;
			request.AddBody(new { P = newPassword });

			apiClient.ExecuteAsync(request, (response) =>
				{
					callback(response.Content);
				});
		}




		public void UpdateMatureFlag(bool wantsMature, string_callback callback)
		{
			RestRequest request = new RestRequest("users/update/mature", Method.PUT);
			request.RequestFormat = DataFormat.Json;
			request.AddBody(new { XXX = wantsMature });

			apiClient.ExecuteAsync(request, (response) =>
				{
					callback(response.Content);
				});
		}

		public void RecoverUser(string userName, string email, string_callback callback)
		{
			RestRequest request = new RestRequest("users/recover/user", Method.POST);
			request.RequestFormat = DataFormat.Json;
			request.AddBody(new { U = userName, E = email });

			apiClient.ExecuteAsync(request, (response) =>
				{
					callback(response.Content);
				});
		}

		public void GetUserPollVote(long blahId, int_callback callback)
		{
			RestRequest request = new RestRequest("blahs", Method.GET);
			request.AddParameter("blahId", blahId);
			request.AddParameter("poll", true);

			apiClient.ExecuteAsync(request, (response) =>
				{
					int theResult = response.Content.FromJson<int>();
					callback(theResult);
				});
		}

		public void SetUserPollVote(long blahId, int theOption, int_callback callback)
		{
			RestRequest request = new RestRequest("blahs", Method.PUT);
			request.RequestFormat = DataFormat.Json;
			request.AddBody(new { v = theOption, id = blahId, poll = true });
			apiClient.ExecuteAsync<int>(request, (response) =>
				{
					callback(response.Data);
				});
		}

		public void GetUserPredictionVote(long blahId, PredictionVote_callback callback)
		{
			RestRequest request = new RestRequest("blahs", Method.GET);
			request.AddParameter("blahId", blahId);
			request.AddParameter("predict", true);

			apiClient.ExecuteAsync<UserPredictionVote>(request, (response) =>
				{
					callback(response.Data);
				});
		}

		public void SetUserPredictionVote(long blahId, string theVote, bool expired, PredictionVote_callback callback)
		{
			RestRequest request = new RestRequest("blahs", Method.PUT);
			request.RequestFormat = DataFormat.Json;
			if (expired)
				request.AddBody(new { t = "post", v = theVote, id = blahId, predict = true });
			else
				request.AddBody(new { t = "pre", v = theVote , id = blahId, predict = true });
			apiClient.ExecuteAsync<UserPredictionVote>(request, (response) =>
				{
					callback(response.Data);
				});
		}


		public void GetUserDescription(long userId, UserDescription_callback callback)
		{
			RestRequest request = new RestRequest("users/descriptor", Method.POST);
			request.RequestFormat = DataFormat.Json;
			request.AddBody(new { I = userId });
			apiClient.ExecuteAsync<UserDescription>(request, (response) =>
				{
					if(response!= null)
						callback(response.Data);
				});

		}

		public void GetCommentAuthorDescriptions(List<long> userIds, UserDescriptionList_callback callback)
		{
			RestRequest request = new RestRequest("users/descriptors", Method.POST);
			request.RequestFormat = DataFormat.Json;
			request.AddBody(new { IDS = userIds });
			apiClient.ExecuteAsync<UserDescriptionList>(request, (response) =>
				{
					callback(response.Data);
				});

		}



		public void FetchFullBlah(long blahId, Blah_callback callback)
		{
			
			RestRequest request = new RestRequest("blahs", Method.GET);
			request.AddParameter ("blahId", blahId);
			apiClient.ExecuteAsync(request, (response) =>
				{
					string blahStr = response.Content;
					Blah theBlah3 = blahStr.FromJson<Blah>();
					callback(theBlah3);
				});
		}

		public void GetBadgeInfo(long badgeId, BadgeRecord_callback callback)
		{
			RestRequest request = new RestRequest("badges/" + badgeId, Method.GET);
			apiClient.ExecuteAsync<BadgeRecord>(request, (response) =>
				{
					callback(response.Data);
				});
		}

		public void GetChannelTypes(ChannelTypeList_callback callback)
		{
			RestRequest request = new RestRequest("groupTypes", Method.GET);
			apiClient.ExecuteAsync<ChannelTypeList>(request, (response) =>
				{
					callback(response.Data);
				});

		}

		public void GetBlahTypes(BlahTypes_callback callback)
		{
			RestRequest request = new RestRequest("blahs/types", Method.GET);
			apiClient.ExecuteAsync<BlahTypeList>(request, (response) =>
				{
					callback(response.Data);
				});

		}

		public void GetInbox(long groupId, Inbox_callback callback)
		{
			RestRequest request = new RestRequest("users/inbox", Method.GET);
			request.AddParameter("groupId", groupId);
			apiClient.ExecuteAsync(request, (response) =>
				{
                    Inbox inbox = null;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
						string respStr = response.Content;
						respStr = respStr.Replace("\"XXX\":null", "\"XXX\":false");
                        inbox = respStr.FromJson<Inbox>();
                        callback(inbox);
                    }
                    else if (response.StatusCode != 0)
                    {
                        callback(null);
                    }
				});
		}
	}
}
