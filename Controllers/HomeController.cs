using EnIyiOzelDers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataBolumu;
using static DataBolumu.AraBirim.UyeSqlBirimi;
using DataBolumu.Models;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web.UI.WebControls;
using System.Dynamic;

namespace EnIyiOzelDers.Controllers
{

    public class HomeController : Controller
    {
        public HomeController()
        {

        }
        public ActionResult Index()
        {

            return View();
        }

        [HttpGet]
        public ActionResult SonuclariGoster(ViewModel model)
        {
            Session["Arama"] = model as DataBolumu.Models.ViewModel;
            TempData["AnasayfaArama1"] = 1;
            return RedirectToAction("Index");
        }

        public PartialViewResult AnasayfaArama()
        {
            List<DersTalepleri> data = DersTalepleriniGetir();
            List<Uye> uyeler = UyeleriGetir();
            var arama = Session["Arama"] as DataBolumu.Models.ViewModel;
            List<DersTalepleri> model = new List<DersTalepleri>();
            List<Uye> selectedUye = new List<Uye>();
            selectedUye = uyeler.FindAll(x => x.Sehir.Contains(arama.Uye.Sehir) && x.UyelikTipi.Contains(arama.Uye.UyelikTipi));
            List<Uye> query = selectedUye.OrderByDescending(x => x.Bakiye).ToList();
            
            foreach(var uye in query)
            {
                model.Add(data.Find(x => x.KullaniciAdi.Contains(uye.KullaniciAdi) && x.AlinmakVerilmekIstenenDersDersler.Contains(arama.DersTalebi.AlinmakVerilmekIstenenDersDersler)));
            }
            
            //model = model.FindAll(x => x.AlinmakVerilmekIstenenDersDersler.Contains(arama.DersTalebi.AlinmakVerilmekIstenenDersDersler));
            

            Session.Remove("Arama");
            return PartialView(model.ToList());
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BakiyeYuklePartial(ViewModel model)
        {
            Uye uyeLogin = new Uye();
            uyeLogin = Session["ActiveUser"] as DataBolumu.Models.Uye;
            int yeniBakiye = uyeLogin.Bakiye + model.Uye.Bakiye;
            if (ModelState.IsValid)
            {
                BakiyeGuncelle(uyeLogin.Id, yeniBakiye);
                Session.Clear();
                return View("Index");
            }
            return View();
        }

        public ActionResult BakiyeYukle()
        {
            TempData["UyeBilgileri1"]=1;
            TempData["UyelikIslemleri1"]=1;
            TempData["BakiyeYukle1"] = 1;
            Uye uyeLogin = new Uye { };
            uyeLogin = Session["ActiveUser"] as DataBolumu.Models.Uye;
            

            if (uyeLogin.UyelikTipi == "Ogrenci")
                return RedirectToAction("OgrenciAnasayfasi");
            else
                return RedirectToAction("OgretmenAnasayfasi");
            
        }

        public ActionResult Contact(Uye model)
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult UyeOl()
        {
            ViewBag.Message = "Üye Ol";

            return View("UyeOl");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UyeOl(UyeModel model)
        {
            
            if (ModelState.IsValid && model.KullaniciAdi != "admin")
            {
                int kayitOlusturuldu = UyeOlustur(model.UyelikTipi, model.KullaniciAdi,
                                        model.EMailAdresi, model.Sifre, model.Adi, model.Soyadi,
                                        model.Sehir, model.Cinsiyet, model.MedeniHal, model.IrtibatNo,
                                        model.DogumTarihi);
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdminBakiyeGuncelle(Uye uye)
        {
            List<Uye> uyeler = UyeleriGetir();
            foreach(var data in uyeler)
            {
                if(uye.Id == data.Id)
                {
                    BakiyeGuncelle(uye.Id, uye.Bakiye);
                    return RedirectToAction("Admin");
                }
            }

            return View("Admin");
        }


        public ActionResult OgrenciAnasayfasi()
        {
            ViewModel viewmodel = new ViewModel();
            Uye uyeLogin = new Uye { };
            uyeLogin = Session["ActiveUser"] as DataBolumu.Models.Uye;

            viewmodel.Uye = uyeLogin;
            if (Session["ActiveUser"] != null)
                return View(viewmodel);
            else
                return RedirectToAction("OgrenciAnasayfasi");
        }


        public ActionResult OgretmenAnasayfasi()
        {
            ViewModel viewmodel = new ViewModel();
            Uye uyeLogin = new Uye { };
            uyeLogin = Session["ActiveUser"] as DataBolumu.Models.Uye;

            viewmodel.Uye = uyeLogin;
            if (Session["ActiveUser"] != null)
                return View(viewmodel);
            else
                return RedirectToAction("OgretmenAnasayfasi");
        }


        public ActionResult Mesajlar()
        {
            Uye uyeLogin = new Uye { };
            uyeLogin = Session["ActiveUser"] as DataBolumu.Models.Uye;
            TempData["Mesajlar1"] = 1;
            TempData["UyeBilgileri1"] = 1;
            
            if(uyeLogin.UyelikTipi == "Ogrenci")
                return RedirectToAction("OgrenciAnasayfasi");
            else
                return RedirectToAction("OgretmenAnasayfasi");

        }

        public ActionResult UyelikIslemleri()
        {
            Uye uyeLogin = new Uye { };
            uyeLogin = Session["ActiveUser"] as DataBolumu.Models.Uye;
            TempData["UyelikIslemleri1"] = 1;
            TempData["UyeBilgileri1"] = 1;

            if (uyeLogin.UyelikTipi == "Ogrenci")
                return RedirectToAction("OgrenciAnasayfasi");
            else
                return RedirectToAction("OgretmenAnasayfasi");
        }
        public ActionResult DersTalebiIslemleri()
        {
            Uye uyeLogin = new Uye { };
            uyeLogin = Session["ActiveUser"] as DataBolumu.Models.Uye;
            TempData["DersTalebiIslemleri1"] = 1;
            TempData["UyeBilgileri1"] = 1;

            if (uyeLogin.UyelikTipi == "Ogrenci")
                return RedirectToAction("OgrenciAnasayfasi");
            else
                return RedirectToAction("OgretmenAnasayfasi");
        }

        public ActionResult KisiselBilgiler()
        {
            Uye uyeLogin = new Uye { };
            uyeLogin = Session["ActiveUser"] as DataBolumu.Models.Uye;
            TempData["KisiselBilgiler1"] = 1;
            TempData["UyeBilgileri1"] = 1;

            if (uyeLogin.UyelikTipi == "Ogrenci")
                return RedirectToAction("OgrenciAnasayfasi");
            else
                return RedirectToAction("OgretmenAnasayfasi");
        }


        public PartialViewResult YeniDersTalebiYaratPartial()
        {
            return PartialView();
        }

        public PartialViewResult GelenMesajlarPartial()
        {
            List<Mesajlar> model = new List <Mesajlar>();
            List<Mesajlar> data = MesajlariGetir();
            Uye uyeLogin = new Uye { };
            uyeLogin = Session["ActiveUser"] as DataBolumu.Models.Uye;

            foreach (Mesajlar mesaj in data)
            {
                if(uyeLogin.KullaniciAdi == mesaj.AliciId)
                {
                    model.Add(mesaj);
                }
            }

            return PartialView(model);

        }

        public PartialViewResult DersTaleplerimPartial()
        {

            List<DersTalepleri> model = new List<DersTalepleri>();
 
            List<DersTalepleri> data = DersTalepleriniGetir();
            Uye uyeLogin = new Uye { };
            uyeLogin = Session["ActiveUser"] as DataBolumu.Models.Uye;
            foreach(DersTalepleri row in data)
            {
                if(row.KullaniciId == uyeLogin.Id)
                {
                    model.Add(row);
                }
            }

            return PartialView(model);
        }

        public ActionResult UyelikBilgileriniGuncelle()
        {
            Uye uyeLogin = new Uye { };
            uyeLogin = Session["ActiveUser"] as DataBolumu.Models.Uye;
            TempData["UyelikBilgileriniGuncelle1"] = 1;
            TempData["KisiselBilgiler1"] = 1;
            TempData["UyeBilgileri1"] = 1;

            if (uyeLogin.UyelikTipi == "Ogrenci")
                return RedirectToAction("OgrenciAnasayfasi");
            else
                return RedirectToAction("OgretmenAnasayfasi");

        }

        public ActionResult DersTaleplerim()
        {
            Uye uyeLogin = new Uye { };
            uyeLogin = Session["ActiveUser"] as DataBolumu.Models.Uye;
            TempData["DersTaleplerim1"] = 1;
            TempData["DersTalebiIslemleri1"] = 1;
            TempData["UyeBilgileri1"] = 1;

            if (uyeLogin.UyelikTipi == "Ogrenci")
                return RedirectToAction("OgrenciAnasayfasi");
            else
                return RedirectToAction("OgretmenAnasayfasi");
        }

        public PartialViewResult MesajGonderPartial()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MesajGonderPartial(ViewModel viewmodel)
        {
            List<Uye> uyeler = UyeleriGetir();
            Uye uyeLogin = new Uye { };
            Random rnd = new Random();
            int mesajId = rnd.Next(1, 999999999);
            uyeLogin = Session["ActiveUser"] as DataBolumu.Models.Uye;
            
            foreach(var uye in uyeler)
            {
                if(uye.KullaniciAdi == viewmodel.Mesaj.AliciId)
                {
                    int mesajOlusturuldu = MesajOlustur(mesajId, viewmodel.Mesaj.AliciId, uyeLogin.KullaniciAdi, viewmodel.Mesaj.Konu, viewmodel.Mesaj.MesajIcerigi);
                    if(uyeLogin.UyelikTipi == "Ogrenci")
                    {
                        return RedirectToAction("OgrenciAnasayfasi");
                    }
                    else
                    {
                        return RedirectToAction("OgretmenAnasayfasi");
                    }
                }
            }
            


            return View();
        }

        public ActionResult MesajGonder()
        {
            Uye uyeLogin = new Uye { };
            uyeLogin = Session["ActiveUser"] as DataBolumu.Models.Uye;
            TempData["Mesajlar1"] = 1;
            TempData["MesajGonder1"] = 1;
            TempData["UyeBilgileri1"] = 1;

            if (uyeLogin.UyelikTipi == "Ogrenci")
                return RedirectToAction("OgrenciAnasayfasi");
            else
                return RedirectToAction("OgretmenAnasayfasi");
        }

        public ActionResult GelenMesajlar()
        {
            Uye uyeLogin = new Uye { };
            uyeLogin = Session["ActiveUser"] as DataBolumu.Models.Uye;
            TempData["Mesajlar1"] = 1;
            TempData["GelenMesajlar1"] = 1;
            TempData["UyeBilgileri1"] = 1;

            if (uyeLogin.UyelikTipi == "Ogrenci")
                return RedirectToAction("OgrenciAnasayfasi");
            else
                return RedirectToAction("OgretmenAnasayfasi");
        }

        public ActionResult YeniDersTalebiYarat()
        {
            Uye uyeLogin = new Uye { };
            uyeLogin = Session["ActiveUser"] as DataBolumu.Models.Uye;
            TempData["YeniDersTalebiYarat1"] = 1;
            TempData["DersTalebiIslemleri1"] = 1;
            TempData["UyeBilgileri1"] = 1;

            if (uyeLogin.UyelikTipi == "Ogrenci")
                return RedirectToAction("OgrenciAnasayfasi");
            else
                return RedirectToAction("OgretmenAnasayfasi");
        }


        public PartialViewResult UyeAnasayfasiBuyukKisim()
        {
            Uye uyeLogin = new Uye { };
            uyeLogin = Session["ActiveUser"] as DataBolumu.Models.Uye;
            return PartialView(uyeLogin);
        }

        public PartialViewResult KisiselBilgilerPartial()
        {
            
            return PartialView();
        }

        public ActionResult UyelikBilgileriniGuncellePartial()
        {

            return PartialView();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YeniDersTalebiYaratPartial(ViewModel model)
        {
            Uye data = Session["ActiveUser"] as DataBolumu.Models.Uye;
            Random rnd = new Random();
            int dersTalebiId = rnd.Next(1,99999999);
            int kullaniciId = data.Id;

            if (ModelState.IsValid)
            {
              int dersTalebiOlusturuldu = DersTalebiOlustur(data.UyelikTipi,dersTalebiId,
                                        model.DersTalebi.DersTalebiKonuBasligi,kullaniciId, model.DersTalebi.DersTalebiIcerigi,
                                        model.DersTalebi.AlinmakVerilmekIstenenDersDersler,data.KullaniciAdi);
                
                if(data.UyelikTipi == "Ogrenci")
                    return RedirectToAction("OgrenciAnasayfasi");
                else
                    return RedirectToAction("OgretmenAnasayfasi");
            }
            return View();
            
        }

        [HttpPost]
        public ActionResult UyelikBilgileriniGuncellePartial(ViewModel model)
        {
            Uye data = Session["ActiveUser"] as DataBolumu.Models.Uye;


            if (ModelState.IsValid)
            {
                UyeGuncelle(data.Id,data.KullaniciAdi,
                                        data.EMailAdresi, model.Uye.Adi, model.Uye.Soyadi,
                                        data.Sehir, data.Cinsiyet, model.Uye.IrtibatNo,
                                        model.Uye.DogumTarihi);
                Session.Clear();
                return RedirectToAction("Index");
            }
            return View();
        }
        

        public PartialViewResult DersTalebiIslemleriPartial()
        {
            DersTalepleri ders = new DersTalepleri { };
            return PartialView(ders);
        }

        public PartialViewResult UyelikIslemleriPartial()
        {
            return PartialView();
        }

        public PartialViewResult MesajlarPartial()
        {
            return PartialView();
        }

        
        public ActionResult Admin()
        {
            List<Uye> data = UyeleriGetir();
            ViewData["Admin"] = data;
            return View();
        }


        public ActionResult UyeGirisi(Uye model)
        {
            
                List<Uye> a = UyeleriGetir();

                foreach (Uye row in a)
                {
                    if (row.KullaniciAdi == model.KullaniciAdi && row.Sifre == model.Sifre)
                    {
                        if (row.UyelikTipi == "Ogrenci")
                        {
                            TempData["Login"] = row;
                            Session["ActiveUser"] = row;
                            ViewData["ActiveUser"] = row.KullaniciAdi;
                            return RedirectToAction("OgrenciAnasayfasi", "Home");

                        }

                        else
                        {   
                            if(row.KullaniciAdi != "admin")
                            {
                                TempData["Login"] = row;
                                Session["ActiveUser"] = row;
                                ViewData["ActiveUser"] = row.KullaniciAdi;
                                return RedirectToAction("OgretmenAnasayfasi", "Home");
                            }
                            else
                            {
                                TempData["Login"] = row;
                                Session["ActiveUser"] = row;
                                ViewData["ActiveUser"] = row.KullaniciAdi;
                                return RedirectToAction("Admin", "Home");
                            }
                                
                        }
                    }
                    
                }


            return View();
            
        }
    
       
        public ActionResult Logout()
        {
            Session.Clear();

            return View("Index");
        }

    }
}