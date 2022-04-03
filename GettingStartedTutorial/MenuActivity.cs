using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;

namespace GettingStartedTutorial
{
    [Activity(Label = "MenuActivity")]
    public class MenuActivity : Activity
    {
        ImageButton imageButton1 = null;
        ImageButton imageButton2 = null;
        ImageButton imageButton3 = null;
        ImageButton imageButton4 = null;
        ImageButton imageButton5 = null;
        ImageButton imageButton6 = null;
        TextView resultado = null;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Menu);
            imageButton1 = FindViewById<ImageButton>(Resource.Id.imageButton1);
            imageButton2 = FindViewById<ImageButton>(Resource.Id.imageButton2);
            imageButton3 = FindViewById<ImageButton>(Resource.Id.imageButton3);
            imageButton4 = FindViewById<ImageButton>(Resource.Id.imageButton4);
            imageButton5 = FindViewById<ImageButton>(Resource.Id.imageButton5);
            imageButton6 = FindViewById<ImageButton>(Resource.Id.imageButton6);
            resultado = FindViewById<TextView>(Resource.Id.resultado);
            // Create your application here
            imageButton1.Click += ImageButton1_Click;
            imageButton2.Click += ImageButton2_Click;
            imageButton3.Click += ImageButton3_Click;
            imageButton4.Click += ImageButton4_Click;
            imageButton5.Click += ImageButton5_Click;
            imageButton6.Click += ImageButton6_Click;
        }

        private  void ImageButton6_Click(object sender, EventArgs e)
        {
            RecibirGet();
        }

        private  void  ImageButton5_Click(object sender, EventArgs e)
        {
            EnviarPost();
        }

        private void ImageButton4_Click(object sender, EventArgs e)
        {
        }

        private void ImageButton3_Click(object sender, EventArgs e)
        {
            EnviarPostLocal(1,"AAAAA","EAN 128");
        }

        private void ImageButton2_Click(object sender, EventArgs e)
        {
            RecibirGetLocal();
        }

        private void ImageButton1_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }
        private async void RecibirGetLocal()
        {
            using (var client = new HttpClient())
            {
                // send a GET request  
                var uri = "https://webappexpoyer.azurewebsites.net/productos";
                var result = await client.GetStringAsync(uri);

                //handling the answer  
                var posts = JsonConvert.DeserializeObject<List<Producto>>(result);

                // generate the output  
                var post = posts.Last();
                resultado.Text = "First post:\n\n" + post.Codigo;
            }
        }

        private async void EnviarPostLocal(int id,string codigo,string tipo)
        {
            using (var client = new HttpClient())
            {
                // Create a new post  
                var novoPost = new Producto
                {
                    Codigo = codigo,
                    Tipo = tipo
                };

                // create the request content and define Json  
                var json = JsonConvert.SerializeObject(novoPost);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                //  send a POST request  
                var uri = "https://webappexpoyer.azurewebsites.net/productos";
                var result = await client.PostAsync(uri, content);

                // on error throw a exception  
                result.EnsureSuccessStatusCode();
                /* Esto esta comentado por si recibe un resultado
                // handling the answer  
                var resultString = await result.Content.ReadAsStringAsync();
                var post = JsonConvert.DeserializeObject<Post>(resultString);

                // display the output in TextView  
                resultado.Text = post.ToString();
                */
            }
        }
        private async void RecibirGet() {
            using (var client = new HttpClient())
            {
                // send a GET request  
                var uri = "http://jsonplaceholder.typicode.com/posts";
                var result = await client.GetStringAsync(uri);

                //handling the answer  
                var posts = JsonConvert.DeserializeObject<List<Post>>(result);

                // generate the output  
                var post = posts.First();
                resultado.Text = "First post:\n\n" + post;
            }
        }

        private async void EnviarPost() {
            using (var client = new HttpClient())
            {
                // Create a new post  
                var novoPost = new Post
                {
                    UserId = 12,
                    Title = "My First Post",
                    Content = "Macoratti .net - Quase tudo para .NET!"
                };

                // create the request content and define Json  
                var json = JsonConvert.SerializeObject(novoPost);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                //  send a POST request  
                var uri = "http://jsonplaceholder.typicode.com/posts";
                var result = await client.PostAsync(uri, content);

                // on error throw a exception  
                result.EnsureSuccessStatusCode();

                // handling the answer  
                var resultString = await result.Content.ReadAsStringAsync();
                var post = JsonConvert.DeserializeObject<Post>(resultString);

                // display the output in TextView  
                resultado.Text = post.ToString();
            }
        }
    }

    public class Producto {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string Codigo { get; set; }
    }

    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Content { get; set; }

        public override string ToString()
        {
            return string.Format(
                "Post Id: {0}\nTitle: {1}\nBody: {2}",
                Id, Title, Content);
        }
    }
}