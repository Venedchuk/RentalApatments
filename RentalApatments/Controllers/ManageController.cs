using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using RentalApatments.Models;
using System.Collections.Generic;

namespace RentalApatments.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var userId = User.Identity.GetUserId();
            IList<string> roles = new List<string> { "Роль не определена" };
            ApplicationUserManager userManager = HttpContext.GetOwinContext()
                                                    .GetUserManager<ApplicationUserManager>();
            ApplicationUser user = userManager.FindByEmail(User.Identity.Name);

            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
            };
            if (user != null)
                model.roles = userManager.GetRoles(user.Id);
            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        [Authorize(Roles = "admin")]
        public ActionResult Initialize()
        {

            using (ApplicationContext db = new ApplicationContext())
            {
                try
                {
                    var input = new TypeAnswer() { Name = "input" };
                    var checkbox = new TypeAnswer() { Name = "checkboxes" };
                    var radiobutton = new TypeAnswer() { Name = "radiobutton" };
                    var textarea = new TypeAnswer() { Name = "textarea" };
                    var option = new TypeAnswer() { Name = "option" };
                    db.TypeAnswers.Add(input);
                    db.TypeAnswers.Add(checkbox);
                    db.TypeAnswers.Add(radiobutton);
                    db.TypeAnswers.Add(option);

                    var numTel = new TypeDescriptionRealty() { Type = input, Name = "Номер телефона" };
                    var description = new TypeDescriptionRealty() { Type = input, Name = "Описание" };
                    db.TypeDescriptionRealties.Add(numTel);
                    db.TypeDescriptionRealties.Add(description);


                    var infrastructure = new TypeDescriptionRealty()
                    {
                        Type = checkbox,
                        Name = "Инфраструктура",
                        Answer = new List<string>()
                        {
                            "Автоматические ворота",
                            "Автомойка",
                            "Автосервис",
                            "Видеонаблюдение",
                            "Въезд по пропускам",
                            "Круглосуточная охрана",
                            "Подвал/погреб",
                            "Смотровая яма",
                            "Шиномонтаж"
                        }
                    };
                    var technikHarakteristick = new TypeDescriptionRealty()
                    {
                        Type = checkbox,
                        Name = "Технические характеристики",
                        Answer = new List<string>()
                        {
                            "Свет",
                            "Вода",
                            "Электричество",
                            "Система пожаротушения",
                            "Отопление"
                        }
                    };
                    var parking = new TypeDescriptionRealty()
                    {
                        Type = radiobutton,
                        Name = "Технические характеристики",
                        Answer = new List<string>() {
                        "Наземная",
                        "Многоуровневая",
                        "Подземная",
                        "На крыше",
                        }
                    };
                    db.TypeDescriptionRealties.Add(infrastructure);
                    db.TypeDescriptionRealties.Add(technikHarakteristick);
                    db.TypeDescriptionRealties.Add(parking);

                    var gsk = new TypeDescriptionRealty()
                    {
                        Type = input
                    };
                    db.TypeDescriptionRealties.Add(gsk);

                    var status = new TypeDescriptionRealty()
                    {
                        Type = radiobutton,
                        Name = "Статус",
                        Answer = new List<string>() {
                        "Кооператив",
                        "Собственность",
                        "По доверенности",
                        }
                    };
                    db.TypeDescriptionRealties.Add(status);

                    var type = new TypeDescriptionRealty()
                    {
                        Type = radiobutton,
                        Name = "Тип",
                        Answer = new List<string>() {
                        "Машиноместо",
                        "Гараж",
                        "Бокс",
                        }
                    };
                    db.TypeDescriptionRealties.Add(type);

                    var adress = new TypeDescriptionRealty()
                    {
                        Type = input,
                        Name = "Адрес",
                    };
                    db.TypeDescriptionRealties.Add(adress);

                    var firstpay = new TypeDescriptionRealty()
                    {
                        Type = option,
                        Name = "Предоплата",
                        Answer = new List<string>() {
                        "Нет",
                        "1 месяц",
                        "2 месяца",
                        "3 месяца",
                        "4 месяца",
                        "5 месяцев",
                        "6 месяцев",
                        "7 месяцев",
                        "8 месяцев",
                        "9 месяцев",
                        "10 месяцев",
                        "11 месяцев",
                        "1 год"
                        }
                    };
                    db.TypeDescriptionRealties.Add(firstpay);

                    var Obplat = new TypeDescriptionRealty()
                    {
                        Type = input,
                        Name = "Обеспечительный платеж",
                    };
                    db.TypeDescriptionRealties.Add(Obplat);

                    var arenHoliday = new TypeDescriptionRealty()
                    {
                        Type = radiobutton,
                        Name = "Арендные каникулы",
                        Answer = new List<string>() {
                        "Да",
                        "Нет"
                        }
                    };
                    db.TypeDescriptionRealties.Add(arenHoliday);

                    var timeRent = new TypeDescriptionRealty()
                    {
                        Type = radiobutton,
                        Name = "Срок аренды",
                        Answer = new List<string>() {
                        "Длительный",
                        "Несколько месяцев"
                        }
                    };
                    db.TypeDescriptionRealties.Add(Obplat);

                    var typeRent = new TypeDescriptionRealty()
                    {
                        Type = radiobutton,
                        Name = "Тип аренды",
                        Answer = new List<string>() {
                        "Прямая аренда",
                        "Суб аренда"
                        }
                    };
                    db.TypeDescriptionRealties.Add(typeRent);

                    var includeStavk = new TypeDescriptionRealty()
                    {
                        Type = checkbox,
                        Name = "В ставку включены",
                        Answer = new List<string>() {
                        "Комуналшьные платежи",
                        "Експлуатационные расходы"
                        }
                    };
                    db.TypeDescriptionRealties.Add(includeStavk);

                    var stavka_za_m2 = new TypeDescriptionRealty()//wtf??
                    {
                        Type = option,
                        Name = "Ставка за м2",
                        Answer = new List<string>() {
                        "В месяц",
                        "В год"
                        }
                    };
                    db.TypeDescriptionRealties.Add(stavka_za_m2);

                    var way = new TypeDescriptionRealty()
                    {
                        Type = option,
                        Name = "Подъездные пути",
                        Answer = new List<string>() {
                        "Асфальтированная дорога",
                        "Грунтовая дорога",
                        "Нет"
                        }
                    };
                    db.TypeDescriptionRealties.Add(way);

                    var water = new TypeDescriptionRealty()
                    {
                        Type = option,
                        Name = "Водоснабжение",
                        Answer = new List<string>() {
                        "На участке",
                        "По границе участка",
                        "Нет"
                        }
                    };
                    db.TypeDescriptionRealties.Add(water);

                    var sewerage = new TypeDescriptionRealty()
                    {
                        Type = option,
                        Name = "Канализация",
                        Answer = new List<string>() {
                        "На участке",
                        "По границе участка",
                        "Нет"
                        }
                    };
                    db.TypeDescriptionRealties.Add(sewerage);

                    var gas = new TypeDescriptionRealty()
                    {
                        Type = option,
                        Name = "Газ",
                        Answer = new List<string>() {
                        "На участке",
                        "По границе участка",
                        "Нет"
                        }
                    };
                    db.TypeDescriptionRealties.Add(gas);

                    var electricity = new TypeDescriptionRealty()
                    {
                        Type = option,
                        Name = "Электричество",
                        Answer = new List<string>() {
                        "На участке",
                        "По границе участка",
                        "Нет"
                        }
                    };
                    db.TypeDescriptionRealties.Add(electricity);


                    var обременения = new TypeDescriptionRealty()
                    {
                        Type = radiobutton,
                        Name = "Наличие обременения",
                        Answer = new List<string>() {
                        "Есть",
                        "Нету"
                        }
                    };
                    db.TypeDescriptionRealties.Add(обременения);

                    var Инвестпроект = new TypeDescriptionRealty()
                    {
                        Type = radiobutton,
                        Name = "Инвестпроект",
                        Answer = new List<string>() {
                        "Есть",
                        "Нету"
                        }
                    };
                    db.TypeDescriptionRealties.Add(Инвестпроект);

                    var currectUse = new TypeDescriptionRealty()
                    {
                        Type = option,
                        Name = "Вид разрешенного использования",
                        Answer = new List<string>() {
                        "Сельскохозяйственное использование",
                        "ИЖС",
                        "МЖС",
                        "Высотная застройка",
                        "Общественное использование объектов капитального строительства",
                        "Деловое управление",
                        "Торговые центры",
                        "Гостиничное обслуживание",
                        "Обслуживание автотранспорта",
                        "Отдых (рекреация)",
                        "Промышленность",
                        "Склады",
                        "Общее пользование территории"
                        },
                        TypeАddition = checkbox,
                        АdditionAnswer = new List<string>()
                        {
                            "Возможно изменить"
                        }
                    };
                    db.TypeDescriptionRealties.Add(currectUse);

                    var categoryArea = new TypeDescriptionRealty()
                    {
                        Type = option,
                        Name = "Категория земли",
                        Answer = new List<string>() {
                        "Не выбрано",
                        "Поселений",
                        "Промышленность",
                        "Сельскохозяйственного назначения",
                        "Промышленности, энергетики, транспорта, связи и иного не сельхоз. назначения"
                        },
                        TypeАddition = checkbox,
                        АdditionAnswer = new List<string>()
                        {
                            "Возможно изменить"
                        }
                    };
                    db.TypeDescriptionRealties.Add(categoryArea);



                    var square = new TypeDescriptionRealty()
                    {
                        Type = input,
                        Name = "Площадь",
                        TypeАddition = option,
                        АdditionAnswer = new List<string>()
                        {
                            "сот",
                            "га"
                        }
                    };
                    db.TypeDescriptionRealties.Add(square);

                    var adress = new TypeDescriptionRealty()
                    {
                        Type = input,
                        Name = "Площадь",
                        TypeАddition = option,
                        АdditionAnswer = new List<string>()
                        {
                            "сот",
                            "га"
                        }
                    };
                    db.TypeDescriptionRealties.Add(adress);









                    var tenant = new TypeDescriptionRealty()
                    {
                        Type = radiobutton,
                        Name = "Состав съемщиков",
                        Answer = new List<string>() {
                        "Любой",
                        "Семья",
                        "Женщина",
                        "Музчина",
                        }
                    };
                    db.TypeDescriptionRealties.Add(tenant);


                    var numberphone = new TypeDescriptionRealty()
                    {
                        Type = input,
                        Name = "Номер телефона"
                    };
                    db.TypeDescriptionRealties.Add(numberphone);

                    var zalog = new TypeDescriptionRealty()
                    {
                        Type = input,
                        Name = "Залог собственнику"
                    };
                    db.TypeDescriptionRealties.Add(zalog);

                    var safepayment = new TypeDescriptionRealty()
                    {
                        Type = input,
                        Name = "Обеспечительный платеж"
                    };
                    db.TypeDescriptionRealties.Add(safepayment);

                    var addictpayment = new TypeDescriptionRealty()
                    {
                        Type = option,
                        Name = "Дополнительные платежи",
                        Answer = new List<string>() {
                        "Оплата всех коммунальных услуг",
                        "Оплата только по счетчикам",
                        "Коммунальныевключены в арендную плату"
                        }
                    };
                    db.TypeDescriptionRealties.Add(addictpayment);

                    var priceforday = new TypeDescriptionRealty()
                    {
                        Type = input,
                        Name = "Цена за сутки",
                        TypeАddition = option,
                        АdditionAnswer = new List<string>() {
                        "Гривны",
                        "Долары",
                        "Евро"
                        },
                        TypeАddition2 = checkbox,
                    };
                db.TypeDescriptionRealties.Add(priceforday);

                    var priceformonth = new TypeDescriptionRealty()
                    {
                        Type = input,
                        Name = "Цена в месяц",
                        TypeАddition = option,
                        АdditionAnswer = new List<string>() {
                        "Гривны",
                        "Долары",
                        "Евро"
                        },
                        TypeАddition2 = checkbox,
                    };
                    db.TypeDescriptionRealties.Add(priceformonth);

                    var price = new TypeDescriptionRealty()
                    {
                        Type = input,
                        Name = "Цена",
                        TypeАddition = option,
                        АdditionAnswer = new List<string>() {
                        "Гривны",
                        "Долары",
                        "Евро"
                        },
                        АdditionAnswer2 = new List<string>() {
                        "Возможен торг",
                        "Возможно в ипотеку",
                        },
                        TypeАddition2 = checkbox,
                    };
                    db.TypeDescriptionRealties.Add(price);











                    db.SaveChanges();
                ViewBag.StatusMessage = "Init succesful";
            }
                catch (Exception a)
            {

                ViewBag.StatusMessage = "Init false";
            }
            finally
            {
                db.SaveChangesAsync();
            }
        }

            return View();
    }

    //
    // POST: /Manage/EnableTwoFactorAuthentication
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> EnableTwoFactorAuthentication()
    {
        await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        if (user != null)
        {
            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        }
        return RedirectToAction("Index", "Manage");
    }

    //
    // POST: /Manage/DisableTwoFactorAuthentication
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DisableTwoFactorAuthentication()
    {
        await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        if (user != null)
        {
            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        }
        return RedirectToAction("Index", "Manage");
    }

    //
    // GET: /Manage/VerifyPhoneNumber
    public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
    {
        var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
        // Send an SMS through the SMS provider to verify the phone number
        return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
    }

    //
    // POST: /Manage/VerifyPhoneNumber
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
        if (result.Succeeded)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
        }
        // If we got this far, something failed, redisplay form
        ModelState.AddModelError("", "Failed to verify phone");
        return View(model);
    }

    //
    // POST: /Manage/RemovePhoneNumber
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> RemovePhoneNumber()
    {
        var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
        if (!result.Succeeded)
        {
            return RedirectToAction("Index", new { Message = ManageMessageId.Error });
        }
        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        if (user != null)
        {
            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        }
        return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
    }

    //
    // GET: /Manage/ChangePassword
    public ActionResult ChangePassword()
    {
        return View();
    }

    //
    // POST: /Manage/ChangePassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
        if (result.Succeeded)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
        }
        AddErrors(result);
        return View(model);
    }

    //
    // GET: /Manage/SetPassword
    public ActionResult SetPassword()
    {
        return View();
    }

    //
    // POST: /Manage/SetPassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
            }
            AddErrors(result);
        }

        // If we got this far, something failed, redisplay form
        return View(model);
    }

    //
    // GET: /Manage/ManageLogins
    public async Task<ActionResult> ManageLogins(ManageMessageId? message)
    {
        ViewBag.StatusMessage =
            message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
            : message == ManageMessageId.Error ? "An error has occurred."
            : "";
        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        if (user == null)
        {
            return View("Error");
        }
        var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
        var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
        ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
        return View(new ManageLoginsViewModel
        {
            CurrentLogins = userLogins,
            OtherLogins = otherLogins
        });
    }

    //
    // POST: /Manage/LinkLogin
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult LinkLogin(string provider)
    {
        // Request a redirect to the external login provider to link a login for the current user
        return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
    }

    //
    // GET: /Manage/LinkLoginCallback
    public async Task<ActionResult> LinkLoginCallback()
    {
        var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
        if (loginInfo == null)
        {
            return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }
        var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
        return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && _userManager != null)
        {
            _userManager.Dispose();
            _userManager = null;
        }

        base.Dispose(disposing);
    }

    #region Helpers
    // Used for XSRF protection when adding external logins
    private const string XsrfKey = "XsrfId";

    private IAuthenticationManager AuthenticationManager
    {
        get
        {
            return HttpContext.GetOwinContext().Authentication;
        }
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error);
        }
    }

    private bool HasPassword()
    {
        var user = UserManager.FindById(User.Identity.GetUserId());
        if (user != null)
        {
            return user.PasswordHash != null;
        }
        return false;
    }

    private bool HasPhoneNumber()
    {
        var user = UserManager.FindById(User.Identity.GetUserId());
        if (user != null)
        {
            return user.PhoneNumber != null;
        }
        return false;
    }

    public enum ManageMessageId
    {
        AddPhoneSuccess,
        ChangePasswordSuccess,
        SetTwoFactorSuccess,
        SetPasswordSuccess,
        RemoveLoginSuccess,
        RemovePhoneSuccess,
        Error
    }

    #endregion
}
}