﻿using KursovaWork.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using KursovaWorkBLL.Services.MainServices.UserService;

namespace KursovaWork.Controllers
{
    /// <summary>
    /// Контролер, що відповідає за вхід користувача в обліковий запис.
    /// </summary>
    public class LogInController : Controller
    {
        /// <summary>
        /// Сервіс для роботи з користувачами
        /// </summary>
        private readonly IUserService _userService;

        /// <summary>
        /// Об'єкт класу ILogger для логування подій 
        /// </summary>
        private readonly ILogger<LogInController> _logger;

        /// <summary>
        /// Ініціалізує новий екземпляр класу <see cref="LogInController"/>.
        /// </summary>
        /// <param name="userService">Сервіс для роботи з користувачами.</param>
        /// <param name="logger">Логгер для запису логів.</param>
        public LogInController(IUserService userService, ILogger<LogInController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Отримує сторінку входу.
        /// </summary>
        /// <returns>Сторінка входу.</returns>
        public IActionResult LogIn()
        {
            _logger.LogInformation("Перехід на сторінку входу");
            return View();
        }

        /// <summary>
        /// Отримує сторінку реєстрації.
        /// </summary>
        /// <returns>Сторінка реєстрації.</returns>
        public IActionResult SignUp()
        {
            _logger.LogInformation("Перехід на сторінку реєстрації");
            return View("~/Views/SignUp/SignUp.cshtml");
        }

        /// <summary>
        /// Обробляє введені користувачем дані входу та аутентифікує користувача.
        /// </summary>
        /// <param name="model">Модель, що містить введені користувачем дані входу.</param>
        /// <returns>Сторінка головного меню або сторінка входу з повідомленням про помилку.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogIn(LogInViewModel model)
        {
            _logger.LogInformation("Вхід у метод перевірки даних входу");
            if (ModelState.IsValid)
            {
                var user = _userService.ValidateUser(model.Email, model.Password);

                if (user != null)
                {

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties).Wait();

                    _logger.LogInformation("Користувач успішно ввійшов в обліковий запис");

                    return Json(new { success = true });
                }

                _logger.LogInformation("Електронну пошту або пароль введено неправильно");
                var error = "Неправильна електронна пошта або пароль.";

                return Json(new { success = false, error  });

            }

            var errors = new
            {
                emailError = ModelState[nameof(LogInViewModel.Email)].Errors.FirstOrDefault()?.ErrorMessage ?? "",
                passwordError = ModelState[nameof(LogInViewModel.Password)].Errors.FirstOrDefault()?.ErrorMessage ?? "",
            };

            _logger.LogInformation("Дані не пройшли валідацію");
            return Json(new { success = false, errors });
        }

    }


}
