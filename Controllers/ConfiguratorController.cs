﻿using KursovaWorkDAL.Entity.Entities.Car;
using Microsoft.AspNetCore.Mvc;
using KursovaWorkDAL.Entity.Entities;
using KursovaWorkBLL.Services.MainServices.CarService;

namespace KursovaWork.Controllers
{
    /// <summary>
    /// Контролер, що відповідає за обробку дій пов'язаних з конфігурацією автомобіля.
    /// </summary>
    public class ConfiguratorController : Controller
    {
        /// <summary>
        /// Сервіс для роботи з автомобілями
        /// </summary>
        private readonly ICarService _carService;

        /// <summary>
        /// Об'єкт класу ILogger для логування подій 
        /// </summary>
        private readonly ILogger<ConfiguratorController> _logger;

        /// <summary>
        /// Об'єкт класу CarInfo? (nullable), який вказує на поточну машину
        /// </summary>
        private static CarInfo _curCar;

        /// <summary>
        /// Об'єкт класу ConfiguratorOptions? (nullable), який вказує на обрані опції в конфігураторі
        /// </summary>
        public static ConfiguratorOptions? options { get; set; }

        /// <summary>
        /// Масив рядків, в якому знаходяться: Марка машини - індекс 0, Модель машини - індекс 1, Рік виробництва - індекс 2
        /// </summary>
        private static string[] _param = new string[3];

        /// <summary>
        /// Ініціалізує новий екземпляр класу <see cref="ConfiguratorController"/>.
        /// </summary>
        /// <param name="carService">Сервіс для роботи з автомобілями.</param>
        /// <param name="logger">Логгер для запису логів.</param>
        public ConfiguratorController(ICarService carService, ILogger<ConfiguratorController> logger)
        {
            _carService = carService;
            _logger = logger;
        }

        /// <summary>
        /// Отримує сторінку з конфігуратором автомобіля.
        /// </summary>
        /// <param name="param1">Параметр 1 (марка автомобіля).</param>
        /// <param name="param2">Параметр 2 (модель автомобіля).</param>
        /// <param name="param3">Параметр 3 (рік виробництва автомобіля).</param>
        /// <returns>Сторінка з конфігуратором автомобіля або сторінка помилки.</returns>
        public IActionResult Configurator(string param1, string param2, string param3)
        {
            _logger.LogInformation("Вхід у функцію переходу на конфігуратор");

            _param = new[] { param1, param2, param3 };

            int year = int.Parse(param3);

            CarInfo car = _carService.GetCarByInfo(param1, param2, year);

            _curCar = car;

            _logger.LogInformation("Машину знайдено, перехід на сторінку конфігуратора");
            return View(car);
        }

        /// <summary>
        /// Обробляє вибрані параметри конфігурації автомобіля.
        /// </summary>
        /// <param name="color">Колір автомобіля.</param>
        /// <param name="transmission">Тип коробки передач автомобіля.</param>
        /// <param name="fuelType">Тип палива автомобіля.</param>
        /// <returns>Сторінка оплати автомобіля або сторінка конфігуратора з повідомленнями про помилки.</returns>
        public IActionResult Submit(string color, string transmission, string fuelType)
        {
            _logger.LogInformation("Вхід у функцію підтвердження конфігурації автомобіля");

            var errors = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(color))
            {
                errors["color"] = "Виберіть колір";
                _logger.LogInformation("Колір не було обрано");
            }

            if (string.IsNullOrEmpty(transmission))
            {
                errors["transmission"] = "Виберіть тип коробки передач";
                _logger.LogInformation("Тип коробки передач не було обрано");
            }

            if (string.IsNullOrEmpty(fuelType))
            {
                errors["fuelType"] = "Виберіть тип палива";
                _logger.LogInformation("Тип палива не було обрано");
            }

            if (errors.Count > 0)
            {
                _logger.LogInformation("Один або більше параметрів не було обрано в конфігураторі");
                return Json(new { errors });
            }

            options = new ConfiguratorOptions()
            {
                Color = color,
                Transmission = transmission,
                FuelType = fuelType
            };

            _logger.LogInformation("Перехід на сторінку оплати автомобіля");
            return Json(new { redirect = Url.Action("Payment", "Payment", new { param1 = _param[0], param2 = _param[1], param3 = _param[2] }) });
        }
    }
}
