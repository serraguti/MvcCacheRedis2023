using MvcCacheRedis.Helpers;
using MvcCacheRedis.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MvcCacheRedis.Services
{
    public class ServiceCacheRedis
    {
        private IDatabase database;

        public ServiceCacheRedis()
        {
            this.database = HelperCacheMultiplexer.GetConnection.GetDatabase();
        }

        //TENDREMOS UN METODO PARA ALMACENAR PRODUCTOS FAVORITOS
        //DENTRO DE CACHE REDIS
        public void AddProductoFavorito(Producto producto)
        {
            //CACHE REDIS FUNCIONA CON CLAVES UNICAS QUE DEBERIAN
            //SER POR USUARIO.
            //NOSOTROS NO TENEMOS LOGIN, TODAS LAS PERSONAS VERAN 
            //LOS MISMOS FAVORITOS.  LO SUYO SERIA CONCATENAR LA CLAVE
            //CON EL ID DEL USUARIO
            //ALMACENAREMOS UNA COLECCION DE PRODUCTOS
            //LO PRIMERO SERA VISUALIZAR SI TENEMOS PRODUCTOS YA ALMACENADOS
            string jsonproductos = this.database.StringGet("favoritos");
            List<Producto> favoritos;
            if (jsonproductos == null)
            {
                //TODAVIA NO HEMOS ALMACENADO NINGUN PRODUCTO
                favoritos = new List<Producto>();
            }
            else
            {
                //RECUPERAMOS LOS PRODUCTOS DE AZURE CACHE REDIS
                favoritos =
                    JsonConvert.DeserializeObject<List<Producto>>(jsonproductos);
            }
            //ALMACENAMOS UN NUEVO FAVORITO
            favoritos.Add(producto);
            //CONVERTIMOS LA COLECCION DE FAVORITOS A FORMATO JSON
            jsonproductos =
                JsonConvert.SerializeObject(favoritos);
            //ALMACENAMOS EN CACHE REDIS LOS NUEVOS PRODUCTOS FAVORITOS
            this.database.StringSet("favoritos", jsonproductos);
        }

        //METODO PARA RECUPERAR TODOS LOS PRODUCTOS FAVORITOS
        public List<Producto> GetProductosFavoritos()
        {
            string jsonproductos = this.database.StringGet("favoritos");
            if (jsonproductos == null)
            {
                return null;
            }
            else
            {
                List<Producto> favoritos =
                    JsonConvert.DeserializeObject<List<Producto>>(jsonproductos);
                return favoritos;
            }
        }

        //METODO PARA ELIMINAR FAVORITOS
        public void DeleteProductoFavorito(int idproducto)
        {
            List<Producto> favoritos = this.GetProductosFavoritos();
            if (favoritos != null)
            {
                //BUSCAMOS EL PRODUCTO A ELIMINAR
                Producto producto = favoritos.FirstOrDefault(z => z.IdProducto == idproducto);
                //ELIMINAMOS DE LA COLECCION
                favoritos.Remove(producto);
                //COMPROBAMOS SI TODAVIA TENEMOS PRODUCTOS O NO
                if (favoritos.Count == 0)
                {
                    //YA NO HAY FAVORITOS, ELIMINAMOS TODO EL CACHE
                    this.database.KeyDelete("favoritos");
                }
                else
                {
                    //SERIALIZAMOS LA COLECCION SIN EL NUEVO PRODUCTO
                    string jsonproductos = JsonConvert.SerializeObject(favoritos);
                    //ALMACENAMOS DE NUEVO LOS PRODUCTOS
                    //TAMBIEN PODEMOS INDICAR EL TIEMPO DE ALMACENAMIENTO
                    this.database.StringSet("favoritos", jsonproductos
                        , TimeSpan.FromMinutes(30));
                }
            }
        }
    }
}
