﻿using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace KursovaWorkBLL.Services.AdditionalServices
{
    /// <summary>
    /// Клас для отримання ідентифікатора користувача.
    /// </summary>
    public class IDRetriever
    {
        /// <summary>
        /// Об'єкт класу ILogger для логування подій 
        /// </summary>
        /// 
        private static readonly ILogger _logger = LoggerFactory.Create(builder => builder.AddConsole())
            .CreateLogger(typeof(IDRetriever));

        /// <summary>
        /// Об'єкт класу IHttpContextAccessor для получення даних про користувача з Cookie
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Ініціалізує новий екземпляр класу IDRetriever.
        /// </summary>
        /// <param name="httpContextAccessor">Об'єкт IHttpContextAccessor.</param>
        public IDRetriever(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Отримує ідентифікатор ввійшовшого користувача.
        /// </summary>
        /// <returns>Ідентифікатор користувача, або 0, якщо користувач не ввійшов.</returns>
        public int GetLoggedInUserId()
        {
            var claimsPrincipal = _httpContextAccessor.HttpContext.User;
            var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                _logger.LogInformation("Користувач є залогіненим");
                return userId;
            }

            _logger.LogInformation("Користувач не є залогіненим");
            return 0;
        }
    }

}
