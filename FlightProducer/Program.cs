var builder = WebApplication.CreateBuilder(args);

// Her fortæller vi ASP.NET Core, hvordan det skal levere en IMessageProducer.
// Ved at bruge AddSingleton genbruger vi den samme instans i hele applikationens levetid.
builder.Services.AddSingleton<IntegrationProject.Service.IMessageProducer, IntegrationProject.Service.MessageProducer>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Flight}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();