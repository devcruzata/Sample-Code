using BAL.Gallery;
using HttpMultipartParser;
using Project.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using TestTalkBloxAPI.Structure;

namespace TestTalkBloxAPI
{
    
    public class GalleryGenreator : IGalleryGenreator
    {
        public GalleryResponse AddGalery(Stream galleryStream)
        {

            GalleryResponse response = new GalleryResponse();
            objResponse Response = new objResponse();

            GalleryManager objGalManager = new GalleryManager();
            Galleries objGallery = new Galleries();
            string auth_token = "";
            try
            {
                var parser = new MultipartFormDataParser(galleryStream);

                // Parse all the fields by name
                var token1 = parser.GetParameterValue("authentication_Token");
                var token2 = parser.GetParameterValue("authentication_token");
                var gName = parser.GetParameterValue("name");
                var gType = parser.GetParameterValue("gallery_Type");
                var gPrice = parser.GetParameterValue("price");
                var gPermission = parser.GetParameterValue("gallery_Permission");

                // Check Whether Req is null or not
                if ((token1 == null && token2 == null) || gName == null || gType == null)
                {
                    response.header.ErrorCode = 500;
                    response.header.ErrorMessage = "Bad Request";
                    response.gallery = null;
                    return response;
                }

                // Now Enter GalleryDetails in db            

                objGallery.name = gName;
                objGallery.gallery_Type = gType;
                objGallery.price = gPrice;
                if (gPermission == "")
                {
                    objGallery.gallery_Permission = "Public";
                }
                else
                {
                    objGallery.gallery_Permission = gPermission;
                }


                objGallery.productIdentifier = "com.nexomni.talkblox.talkblox" + Regex.Replace(gName, "[^a-zA-Z0-9_]+", "");

                if (gPrice == "")
                {
                    objGallery.isBuy = false;
                }
                else
                {
                    objGallery.isBuy = true;
                }

                if (token1 != null)
                {
                    auth_token = token1;
                }

                if (token2 != null)
                {
                    auth_token = token2;
                }
                Response = objGalManager.AddGallery(objGallery, auth_token);

                if (Response.ErrorCode == 0)
                {
                    if (Response.ErrorMessage != "Invalid Authentication Token")
                    {
                        // Now Save All Media File On Server
                        objGallery.id = Response.ResponseData.Tables[0].Rows[0][0].ToString();

                        // Files are in list parse and save them one by one
                        foreach (var file in parser.Files)
                        {
                            var temp = file.FileName.Split('.');
                            string filename = Guid.NewGuid() + "." + temp[temp.Length - 1];

                            Stream data = file.Data;
                            string ThumbnailName = file.FileName;

                            if (gType == "sound")
                            {
                                UploadAsStream(data, HostingEnvironment.MapPath(ConfigurationManager.AppSettings["galAudioUploadDirectory"].ToString()) + filename);
                            }
                            else if (gType == "video")
                            {
                                UploadAsStream(data, HostingEnvironment.MapPath(ConfigurationManager.AppSettings["galVideoUploadDirectory"].ToString()) + filename);
                                string fpath = HostingEnvironment.MapPath(ConfigurationManager.AppSettings["galVideoUploadDirectory"].ToString()) + filename;
                                string thpath = HostingEnvironment.MapPath(ConfigurationManager.AppSettings["galThumbUploadDirectory"].ToString()) + Guid.NewGuid() + "." + "jpg";
                                genreateThumb(fpath, thpath);
                            }
                            else if (gType == "image")
                            {
                                UploadAsStream(data, HostingEnvironment.MapPath(ConfigurationManager.AppSettings["galImageUploadDirectory"].ToString()) + filename);
                            }
                            else if (gType == "backgroundimage")
                            {
                                UploadAsStream(data, HostingEnvironment.MapPath(ConfigurationManager.AppSettings["galBackImageUploadDirectory"].ToString()) + filename);
                            }
                            else
                            {
                                UploadAsStream(data, HostingEnvironment.MapPath(ConfigurationManager.AppSettings["galSoundTrackUploadDirectory"].ToString()) + filename);
                            }

                            Response = objGalManager.AddGalleryMedia(objGallery.id, filename, ThumbnailName, gType);

                            if (Response.ErrorCode == 0)
                            {
                                response.header.ErrorCode = 200;
                                response.header.ErrorMessage = "Success";
                                response.gallery.id = objGallery.id;
                                response.gallery.name = objGallery.name;
                                response.gallery.galleryType = objGallery.gallery_Type;
                            }
                            else
                            {
                                response.header.ErrorCode = 501;
                                response.header.ErrorMessage = "An Error Occured In Uploading Media , Please Try Again";
                                response.gallery = null;
                                return response;
                            }
                        }

                        response.header.ErrorCode = 200;
                        response.header.ErrorMessage = "Success";
                        response.gallery.id = objGallery.id;
                        response.gallery.name = objGallery.name;
                        response.gallery.galleryType = objGallery.gallery_Type;
                        // response.response.TimeStamp = DateTime.Now.ToUniversalTime().ToString("u");
                    }
                    else
                    {
                        response.header.ErrorCode = 501;
                        response.header.ErrorMessage = Response.ErrorMessage;
                        response.gallery = null;
                        return response;
                    }
                }
                else
                {
                    response.header.ErrorCode = 501;
                    response.header.ErrorMessage = "An Error Occured , Please Try Again";
                    response.gallery = null;
                    return response;
                }

            }
            catch (Exception ex)
            {
                response.header.ErrorCode = 501;
                response.header.ErrorMessage = "Error Occured : " + ex.Message.ToString();
                response.gallery = null;
                return response;
            }

            return response;
        }

        #region Utility Method


        public async void UploadAsStream(Stream fileData, string fPath)
        {
            using (FileStream fileStream = File.Create(fPath, (int)fileData.Length))
            {
                byte[] bytesInStream = new byte[fileData.Length];
                await fileData.ReadAsync(bytesInStream, 0, bytesInStream.Length);
                await fileStream.WriteAsync(bytesInStream, 0, bytesInStream.Length);
            }

        }

        public void UploadFileAsByte(string fPath, byte[] data)
        {
            string filePath = fPath;
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            fs.Write(data, 0, data.Length);
            fs.Close();
        }

        public void genreateThumb(string fPath, string tPath)
        {

            try
            {

                var processInfo = new ProcessStartInfo();
                processInfo.FileName = "\"" + HttpContext.Current.Server.MapPath("/ThumbHelper/ffmpeg.exe") + "\"";
                processInfo.Arguments = string.Format("-ss {0} -i {1} -f image2 -vframes 1 -y {2}", 5, "\"" + fPath + "\"", "\"" + tPath + "\"");
                processInfo.CreateNoWindow = true;
                processInfo.UseShellExecute = false;
                using (var process = new Process())
                {
                    process.StartInfo = processInfo;
                    process.Start();
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }
        # endregion
    }
}
