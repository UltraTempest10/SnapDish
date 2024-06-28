using Autofac;
using Autofac.Extensions.DependencyInjection;
using SqlSugar;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Server.config
{
    public static class HostBuilderExtend
    {
        public static void Register(this WebApplicationBuilder app)
        {
            app.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            app.Host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                #region 注册sqlsugar
                builder.Register<ISqlSugarClient>(context =>
                {
                    SqlSugarClient db = new SqlSugarClient(new ConnectionConfig
                    {
                        ConnectionString = "server=121.196.246.253;Database=food;Uid=food;Pwd=.Net1234",
                        DbType = DbType.MySql,
                        IsAutoCloseConnection = true
                    });

                    db.Aop.OnLogExecuted = (sql, par) => {


                        Console.WriteLine("\r\n");
                        Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}，sql语句：{sql}");
                        Console.WriteLine("========================================================");
                    };

                    db.Aop.OnError = (e) =>
                    {
                        Console.WriteLine(e + $" 执行SQL出错：{e.Message}");
                        Console.WriteLine("********************************************************");
                    };

                    return db;
                });
                #endregion

                //注册service绑定
                builder.RegisterModule(new AutofacModuleRegister());
            });


            //jwt相关
            var configuration = app.Configuration;
            app.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true, //是否验证Issuer
                    ValidIssuer = configuration["Authentication:Issuer"], //发行人Issuer
                    ValidateAudience = true, //是否验证Audience
                    ValidAudience = configuration["Authentication:Audience"], //订阅人Audience
                    ValidateIssuerSigningKey = true, //是否验证SecurityKey
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:SecretKey"])), //SecurityKey
                    ValidateLifetime = true, //是否验证失效时间
                    ClockSkew = TimeSpan.FromSeconds(30), //过期时间容错值，解决服务器端时间不同步问题（秒）
                    RequireExpirationTime = true,
                };
            });
        }
    }
}
