using MageritHealth.Data;
using MageritHealth.Helpers;
using MageritHealth.Models;
using MageritHealth.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MageritHealth.Repositories
{
    public class UsuariosRepository : IUsuariosRepository
    {
        private readonly MageritHealthDbContext context;

        public UsuariosRepository(MageritHealthDbContext context)
        {
            this.context = context;
        }

        public async Task<Usuario> LoginUsuarioAsync(string email, string password)
        {
            Usuario user = await this.context.Usuarios.Include(u => u.Especialidad).FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || !user.Activo)
            {
                return null;
            }

            //if (user.Pass != password)
            //{
            //    return null;
            //}
            //else
            //{
            //    return user;
            //}

            Credencial credencial = await this.context.Credenciales.FirstOrDefaultAsync(c => c.IdUsuario == user.IdUsuario);

            if (credencial == null)
            {
                return null;
            }

            byte[] hashedPassword = CryptographyHelper.EncryptPassword(password, credencial.Salt);

            if (ToolsHelper.CompareArrays(hashedPassword, credencial.PasswordHash))
            {
                return user;
            }
            else
            {
                return null;
            }

        }

        public async Task<Usuario> GetUsuarioByIdAsync(int id)
        {
            return await this.context.Usuarios
                .Include(u => u.Especialidad).Include(u => u.CitasComoDoctor).Include(u => u.CitasComoPaciente).Include(u => u.Antecedentes)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);
        }

        public async Task<List<Usuario>> GetUsuariosByDniYRolAsync(string dni, string rol)
        {
            var query = this.context.Usuarios
                .Include(u => u.Especialidad)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(dni))
            {
                query = query.Where(u => u.Dni.Contains(dni));
            }

            if (!string.IsNullOrWhiteSpace(rol))
            {
                query = query.Where(u => u.Rol == rol);
            }

            return await query.ToListAsync();
        }

        public async Task<List<Usuario>> GetListaUsuariosByRolAsync(string rol)
        {
            return await this.context.Usuarios.Where(u => u.Rol == rol && u.Activo).ToListAsync();
        }

        public async Task<int> GetRecuentoUsuariosByRolAsync(string rol)
        {
            return await this.context.Usuarios.CountAsync(u => u.Rol == rol && u.Activo);
        }

        public async Task<List<Usuario>> GetListaPacientesByIdDoctorAsync(int idDoctor) // en el futuro cambiara
        {
            return await this.context.Usuarios.Where(u => u.Rol == "paciente" && u.CitasComoPaciente.Any(c => c.IdDoctor == idDoctor) && u.Activo).Distinct().ToListAsync();
        }

        public async Task<List<Usuario>> GetListaDoctoresByEspecialidadAsync(int especialidad)
        {
            return await this.context.Usuarios.Where(u => u.Rol == "doctor" && u.IdEspecialidad == especialidad && u.Activo).ToListAsync();
        }

        public async Task InsertUsuarioAsync(Usuario user, string password)
        {
            // Añadir el nuevo usuario a la BBDD
            int maxId = await this.context.Usuarios.MaxAsync(u => (int?)u.IdUsuario) ?? 0; // para asegurar que si la tabla está vacía, el primer id sea 1
            user.IdUsuario = maxId + 1;

            // Generación de password segura
            string salt = ToolsHelper.GenerateSalt();
            byte[] hashedPassword = CryptographyHelper.EncryptPassword(password, salt);


            // Creación de la credencial asociada al usuario
            int maxCredencial = await this.context.Credenciales.MaxAsync(c => (int?)c.IdCredencial) ?? 0;
            Credencial credencial = new Credencial
            {
                IdCredencial = maxCredencial + 1,
                IdUsuario = user.IdUsuario,
                PasswordHash = hashedPassword,
                Salt = salt
            };
            this.context.Usuarios.Add(user);
            this.context.Credenciales.Add(credencial);

            await this.context.SaveChangesAsync();
        }

        public async Task UpdateUsuarioAsync(Usuario changedUser)
        {
            Usuario savedUser = await this.context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == changedUser.IdUsuario);
            if (savedUser != null)
            {
                Credencial credencial = await this.context.Credenciales.FirstOrDefaultAsync(c => c.IdUsuario == savedUser.IdUsuario);
                string salt = ToolsHelper.GenerateSalt();
                credencial.Salt = salt;
                credencial.PasswordHash = CryptographyHelper.EncryptPassword(changedUser.Pass, salt);
                this.context.Entry(savedUser).CurrentValues.SetValues(changedUser);
                await this.context.SaveChangesAsync();
            }
        }

        public async Task UpdateDatosUsuarioAsync(int idUsuario, string telefono, string email, string password)
        {
            Usuario usuario = await this.context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

            if (usuario != null)
            {
                usuario.Telefono = telefono;
                usuario.Email = email;
                if (password != null)
                {
                    usuario.Pass = password;
                    Credencial credencial = await this.context.Credenciales.FirstOrDefaultAsync(c => c.IdUsuario == idUsuario);
                    if (credencial != null)
                    {
                        string salt = ToolsHelper.GenerateSalt();
                        credencial.Salt = salt;
                        credencial.PasswordHash = CryptographyHelper.EncryptPassword(password, salt);
                    }
                }

                await this.context.SaveChangesAsync();
            }
        }

        public async Task UpdatePasswordUsuarioAsync(int idUsuario, string oldPassword, string newPassword)
        {
            Credencial credencial = await this.context.Credenciales.FirstOrDefaultAsync(c => c.IdUsuario == idUsuario);
            if (credencial != null)
            {
                byte[] oldHash = CryptographyHelper.EncryptPassword(oldPassword, credencial.Salt);
                if (ToolsHelper.CompareArrays(oldHash, credencial.PasswordHash))
                {
                    string newSalt = ToolsHelper.GenerateSalt();
                    byte[] hashedPassword = CryptographyHelper.EncryptPassword(newPassword, newSalt);
                    credencial.PasswordHash = hashedPassword;
                    credencial.Salt = newSalt;
                    await this.context.SaveChangesAsync();
                }
            }
        }

        public async Task ResetPasswordUsuarioAsync(int idUsuario, string newPassword) // una password generica aleatoria, metodo usado por admins para resetear.
        {
            Credencial credencial = await this.context.Credenciales.FirstOrDefaultAsync(c => c.IdUsuario == idUsuario);

            if (credencial != null)
            {
                /* TEMPORAL */
                Usuario usuario = await this.context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
                usuario.Pass = newPassword;
                /* TEMPORAL */

                string salt = ToolsHelper.GenerateSalt();
                byte[] hashedPassword = CryptographyHelper.EncryptPassword(newPassword, salt);
                credencial.PasswordHash = hashedPassword;
                credencial.Salt = salt;

                await this.context.SaveChangesAsync();
            }
        }

        public async Task EnableUsuarioAsync(int idUsuario)
        {
            Usuario user = await this.context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
            if (user != null)
            {
                user.Activo = true;
                await this.context.SaveChangesAsync();
            }
        }

        public async Task DisableUsuarioAsync(int idUsuario)
        {
            Usuario user = await this.context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

            if (user != null)
            {
                user.Activo = false;
                await this.context.SaveChangesAsync();
            }
        }

        public async Task<List<Especialidad>> GetListaEspecialidadesAsync()
        {
            return await this.context.Especialidades.Include(e => e.Doctores).ToListAsync();
        }

        public async Task<Especialidad> GetEspecialidadByIdAsync(int idEspecialidad)
        {
            return await this.context.Especialidades.FirstOrDefaultAsync(e => e.IdEspecialidad == idEspecialidad);
        }

        public async Task InsertEspecialidadAsync(string especialidad)
        {
            int maxId = await this.context.Especialidades.MaxAsync(e => (int?)e.IdEspecialidad) ?? 0;
            Especialidad nuevaEspecialidad = new Especialidad
            {
                NombreEspecialidad = especialidad,
                IdEspecialidad = maxId + 1
            };

            await this.context.Especialidades.AddAsync(nuevaEspecialidad);
            await this.context.SaveChangesAsync();
        }

        public async Task DeleteEspecialidadAsync(int idEspecialidad)
        {
            Especialidad especialidad = await this.context.Especialidades.FirstOrDefaultAsync(e => e.IdEspecialidad == idEspecialidad);

            bool enUso = await this.context.Usuarios.AnyAsync(u => u.IdEspecialidad == idEspecialidad);

            if (especialidad != null && !enUso)
            {
                this.context.Especialidades.Remove(especialidad);
                await this.context.SaveChangesAsync();
            }
        }

        public async Task<Usuario> GetUsuarioByEmailAsync(string email)
        {
            return await this.context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
