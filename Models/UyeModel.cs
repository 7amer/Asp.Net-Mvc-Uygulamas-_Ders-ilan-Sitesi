using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;



namespace EnIyiOzelDers.Models
{

    public class UyeModel
    {
        
        [Display(Name = "Uyelik Tipi")]
        public string UyelikTipi { get; set; }

        [Display(Name = "Kullanıcı Adı")]
        [Required(ErrorMessage ="Kullanici Adi kismi bos birakilamaz.")]
        public string KullaniciAdi { get; set; }

        [Display(Name = "Sifre")]
        [DataType(DataType.Password)]
        [StringLength(49,MinimumLength=8,ErrorMessage ="Sifre en az 8 haneli olmali.")]
        [Required(ErrorMessage = "Bir sifreye ihtiyaciniz var.")]
        public string Sifre { get; set; }

        [Display(Name = "Sifre Onay")]
        [DataType(DataType.Password)]
        [Compare("Sifre",ErrorMessage ="Sifreler eslesmiyor, lutfen kontrol ediniz")]
        public string SifreOnay { get; set; }

        [Display(Name = "Email Adresi")]
        [Required(ErrorMessage = "Bir emaile ihtiyaciniz var.")]
        [DataType(DataType.EmailAddress)]
        public string EMailAdresi { get; set; }
        
        
        [Display(Name = "Email Adresi Onay")]
        [Compare("EMailAdresi",ErrorMessage ="Emailler eslesmiyor, lutfen kontrol ediniz.")]
        public string EMailAdresiOnay { get; set; }


        [Display(Name = "Adi")]
        [Required(ErrorMessage = "Ad kismi bos birakilamaz")]
        public string Adi { get; set; }

        [Display(Name = "Soyadi")]
        [Required(ErrorMessage = "Soyad kismi bos birakilamaz")]
        public string Soyadi { get; set; }

        [Display(Name ="Dogum Tarihi")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage ="Lutfen gecerli bir dogum tarihi giriniz.")]
        public string DogumTarihi { get; set; }

        public string Sehir { get; set; }
        public string Cinsiyet { get; set; }
        public string MedeniHal { get; set; }

        [Display(Name = "Irtibat No")]
        [Required(ErrorMessage ="Lutfen gecerli bir telefon numarasi giriniz.")]
        public string IrtibatNo { get; set; }


    }
}