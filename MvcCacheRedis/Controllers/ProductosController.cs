using Microsoft.AspNetCore.Mvc;
using MvcCacheRedis.Models;
using MvcCacheRedis.Repositories;
using MvcCacheRedis.Services;

namespace MvcCacheRedis.Controllers
{
    public class ProductosController : Controller
    {
        private RepositoryProductos repo;
        private ServiceCacheRedis service;

        public ProductosController
            (RepositoryProductos repo, ServiceCacheRedis service)
        {
            this.repo = repo;
            this.service = service;
        }

        public IActionResult Index()
        {
            List<Producto> productos = this.repo.GetProductos();
            return View(productos);
        }

        public IActionResult Details(int id)
        {
            Producto producto = this.repo.FindProducto(id);
            return View(producto);
        }

        public IActionResult Favoritos()
        {
            List<Producto> favoritos = this.service.GetProductosFavoritos();
            return View(favoritos);
        }

        public IActionResult SeleccionarFavorito(int id)
        {
            //BUSCAMOS EL PRODUCTO A ALMACENAR, QUE LO 
            //TENEMOS EN EL REPOSITORY XML
            Producto producto = this.repo.FindProducto(id);
            //ALMACENAMOS EL PRODUCTO EN AZURE
            this.service.AddProductoFavorito(producto);
            ViewData["MENSAJE"] = "Producto almacenado: " + producto.Nombre;
            return RedirectToAction("Details", new { id = id });
        }

        public IActionResult DeleteFavorito(int id)
        {
            this.service.DeleteProductoFavorito(id);
            return RedirectToAction("Favoritos");
        }
    }
}
