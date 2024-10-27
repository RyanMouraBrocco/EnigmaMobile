using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using EnigmaClass;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Enigma
{
    public partial class Conteudos_Page : ContentPage
    {
        Server server = new Server();
        HttpClient client = new HttpClient();
        List<Conteudo> conteudos = new List<Conteudo>();
        Materia materia = new Materia();
        ColumnDefinition c1 = new ColumnDefinition();
        bool pri = true;
        int qtd = 0;
        int l = 0;
        bool isbusy = false;

        public Conteudos_Page(Materia materia)
        {
            InitializeComponent();
            this.materia = materia;
            Title = "Conteúdos - " + materia.Nome;
            _grid.ColumnDefinitions.Add(c1);
            Controle.data = DateTime.Now;
            Controle.desempenho = new Desempenho { Materia = materia, Usuario = new Usuario {ID = UsuarioAtual.ID} };
            Controle.verificar = true;

        }
        public async void CarregarAsync()
        {
           try
           {
                _grid.RowDefinitions.Clear();
                _grid.ColumnDefinitions.Clear();
                var resposta = await client.GetStringAsync(server.Servidor + "/api/Conteudo/bymateria/" + materia.ID);
                conteudos = JsonConvert.DeserializeObject<List<Conteudo>>(resposta);
                foreach (var item in conteudos.OrderBy(x=>x.Ordem))
                {
                    RowDefinition d = new RowDefinition();
                    d.Height = 100;
                    _grid.RowDefinitions.Add(d);
                    var botao = new Button
                    {
                        Text = item.ID.ToString() + "-" + item.Nome,
                        BackgroundColor = Color.Transparent,
                        TextColor = Color.White,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand
                    };
                    botao.Clicked += async (sender, e) =>
                    {
                        if (isbusy==false)
                        {
                            isbusy = true;
                            frm_carregando.IsVisible = true;
                            Button a = sender as Button;
                            try
                            {
                                var r = await client.GetStringAsync(server.Servidor + "/api/Conteudo/byid/" + a.Text.Substring(0, a.Text.IndexOf("-", StringComparison.Ordinal)));
                                Conteudo conteudo = JsonConvert.DeserializeObject<Conteudo>(r);
                                frm_carregando.IsVisible = false;
                                await Navigation.PushAsync(new ConteudoView_Page(conteudo));
                                isbusy = false;
                            }
                            catch
                            {
                                frm_carregando.IsVisible = false;
                                isbusy = false;
                                await DisplayAlert("ERRO", "Sem conexão", "OK");
                            }
                        }
                       

                    };
                    if (item.Imagem != null)
                    {
                        MemoryStream ms = new MemoryStream(item.Imagem);
                        Image img = new Image();
                        img.Source = ImageSource.FromStream(() => { return ms; });
                        img.HorizontalOptions = LayoutOptions.FillAndExpand;
                        img.VerticalOptions = LayoutOptions.FillAndExpand;
                        _grid.Children.Add(img, 0, l);
                    }
                    else
                    {
                        botao.BackgroundColor = Color.FromHex("#000449");
                    }
                    _grid.Children.Add(botao, 0, l);
                    l += 1;
                    qtd += 1;
                }

            }
            catch
            {
                await DisplayAlert("ERRO", "Sem conexão", "OK");
            }

        }
        protected  override void OnAppearing()
        {
            base.OnAppearing();
            if (pri == true)
            {
                frm_carregando.IsVisible = true;
                CarregarAsync();
                frm_carregando.IsVisible = false;
                pri = false;
            }
        }

    }
}
