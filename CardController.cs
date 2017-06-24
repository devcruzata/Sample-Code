using BAL.Cards;
using Ionic.Zip;
using Project.ViewModel;
using Project.Web.Common;
using Project.Web.Filters;
using Project.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Web.Controllers.Cards
{
    public class CardController : Controller
    {

        CardManager objCardManager = new CardManager();
        SessionHelper session;

        [Authorize]
        [HttpPost]
        [SessionTimeOut]
        public ActionResult GenreateCards(CardModel objCardModel)
        {
            session = new SessionHelper();
            Barcode.BarcodeGenreator objBarcode = new Barcode.BarcodeGenreator();
            System.Drawing.Image barcodeImage = null;
            BarcodeLib.Barcode b = new BarcodeLib.Barcode();
            List<string> cards = new List<string>();


            List<TextValue> OrganijationList = new List<TextValue>();
            OrganijationList = BAL.Common.UtilityManager.GetMerchantsForDropDown();

            List<SelectListItem> list2 = new List<SelectListItem>();
            list2.Add(new SelectListItem { Value = "0", Text = "Choose A Organijation" });

            foreach (var org in OrganijationList)
            {
                list2.Add(new SelectListItem { Value = org.Value, Text = org.Text });
            }

            try
            {
                cards = objCardManager.CardNoGenreator(objCardModel.Organijation, objCardModel.ValidFrom, objCardModel.ValidTo, objCardModel.Quantity, objCardModel.CardNo_Prefix, session.UserSession.Username);
                foreach (var card in cards)
                {
                    b.IncludeLabel = true;
                    barcodeImage = b.Encode(BarcodeLib.TYPE.UPCA, card, System.Drawing.ColorTranslator.FromHtml("#000000"), System.Drawing.ColorTranslator.FromHtml("#FFFFFF"), 300, 100);
                    b.SaveImage(Server.MapPath(ConfigurationManager.AppSettings["Barcode_dir"]) + card + ".JPG", BarcodeLib.SaveTypes.JPG);
                }
                string path = Server.MapPath(ConfigurationManager.AppSettings["Barcode_dir"]) + objCardModel.Barcode_dir;
                string[] filePaths = Directory.GetFiles(Server.MapPath(ConfigurationManager.AppSettings["Barcode_dir"]));
                using (ZipFile zip = new ZipFile())
                {

                    zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                    zip.AddDirectoryByName("Files");
                    foreach (string filePath in filePaths)
                    {
                        zip.AddFile(filePath, "Barcodes");
                    }
                    Response.Clear();
                    Response.BufferOutput = false;
                    string zipName = String.Format("Barcodes_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
                    Response.ContentType = "application/zip";
                    Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                    zip.Save(Response.OutputStream);
                    Response.End();
                }
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }
                ViewBag.Organijation = list2;
                return View();
            }
            catch (Exception ex)
            {
                BAL.Common.LogManager.LogError("GenreateCards", 1, Convert.ToString(ex.Source), Convert.ToString(ex.Message), Convert.ToString(ex.StackTrace));
                ViewBag.Organijation = list2;
                return View();
            }
        }

    }
}
