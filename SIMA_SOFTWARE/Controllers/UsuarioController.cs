using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMA_SOFTWARE.Data;
using SIMA_SOFTWARE.Models;
using SIMA_SOFTWARE.Models.ViewModels;
using SIMA_SOFTWARE.Services;

namespace SIMA_SOFTWARE.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly EmailService _emailService;
        private readonly SimaDbContext _context;

        public UsuarioController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 EmailService emailService,
                                 SimaDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel Usuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = await _signInManager.PasswordSignInAsync(Usuario.Email, Usuario.Clave, Usuario.Recordarme, lockoutOnFailure: false);
                if (resultado.Succeeded)
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Inicio de sesión fallido");
                }
            }
            return View(Usuario);
        }

        [HttpGet]
        public IActionResult RecuperarConfirmacion()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RecuperarCuenta()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecuperarCuenta(RecuperoCuentaViewModel Usuario)
        {
            if (!ModelState.IsValid)
                return View(Usuario);

            var user = await _userManager.FindByEmailAsync(Usuario.Email);

            if (user != null)
            {
                // 🔥 Generar código OTP
                var codigo = new Random().Next(100000, 999999).ToString();

                // 🔥 (OPCIONAL PRO) eliminar códigos anteriores
                var anteriores = _context.CodigosRecuperacion
                    .Where(x => x.UserId == user.Id && !x.Usado);

                _context.CodigosRecuperacion.RemoveRange(anteriores);

                // 🔥 Guardar nuevo código
                var otp = new CodigoRecuperacion
                {
                    UserId = user.Id,
                    Codigo = codigo,
                    Expiracion = DateTime.Now.AddMinutes(10),
                    Usado = false
                };

                _context.CodigosRecuperacion.Add(otp);
                await _context.SaveChangesAsync();

                // 🔥 Enviar email
                await _emailService.EnviarEmail(
                    Usuario.Email,
                    "Código de recuperación",
                    $"Tu código OTP es: <b>{codigo}</b>"
                );
            }

            return RedirectToAction("ValidarOTP",new { email = Usuario.Email });
        }

        [HttpGet]
        public IActionResult ValidarOtp(string email)
        {
            return View(new ValidarOTPViewModel { Email = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidarOtp(ValidarOTPViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.NuevaPassword != model.ConfirmarPassword)
            {
                ModelState.AddModelError("", "Las contraseñas no coinciden");
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Error");
                return View(model);
            }

            var otp = _context.CodigosRecuperacion
                .Where(x => x.UserId == user.Id &&
                            x.Codigo == model.Codigo &&
                            !x.Usado &&
                            x.Expiracion > DateTime.Now)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            if (otp == null)
            {
                ModelState.AddModelError("", "Código inválido o expirado");
                return View(model);
            }

            // 🔥 marcar como usado
            otp.Usado = true;
            await _context.SaveChangesAsync();

            // 🔥 resetear contraseña
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(
                user,
                token,
                model.NuevaPassword
            );

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Error al cambiar contraseña");
                return View(model);
            }

            return RedirectToAction("Login");
        }








        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public  async Task<IActionResult> Registro(RegistroViewModel usuario)
        {
            if (ModelState.IsValid)
            {
                //Logica para registrar al usuario
                var nuevoUsuario = new ApplicationUser
                {
                    UserName = usuario.Email,
                    Email = usuario.Email,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    ImagenUrlPerfil = "default-profile.png" // Asignar una imagen de perfil predeterminada
                };

                var resultado = await _userManager.CreateAsync(nuevoUsuario, usuario.Clave);

                if (resultado.Succeeded)
                {
                    await _signInManager.SignInAsync(nuevoUsuario, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(usuario);
        }
        

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
