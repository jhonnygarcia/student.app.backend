using Core.DbModel.Contexts;
using Core.Services;
using Core.Services.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(expression =>
            {
                expression.AddProfile(new MappingProfile());
            });
            services.AddTransient<IStudentServices, StudentServices>();
            services.AddTransient<IScoreServices, ScoreServices>();
            services.AddTransient<ISubjectServices, SubjectServices>();
            services.AddTransient<ITeacherServices, TeacherServices>();
            services.AddTransient<IClassServices, ClassServices>();
            services.AddDbContext<StudentContext>(o => o.UseSqlServer(configuration["ConnectionStrings:DbConnection"]));
            return services;
        }
    }
}
