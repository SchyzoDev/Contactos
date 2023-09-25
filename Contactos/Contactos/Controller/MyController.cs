using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Contactos.View;
using Contactos.Model;
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace Contactos.Controller
{
    public class MyController:UIElement
    {
        MainWindow main;
        public Cmd cmdnavegar { get; set; }
        public Cmd cmdinserirContacto { get; set; }
        public Cmd cmdapagaContacto { get; set; }
        public Cmd cmdguardaContacto { get; set; }
        public Cmd cmdguardaImagem { get; set; }
        public Cmd Fechar { get; set; }

        public MyController()
        {
            main = (MainWindow)App.Current.MainWindow;
            cmdnavegar = new Cmd(Navega, podeNavegar);
            cmdinserirContacto = new Cmd(inserirContacto, (inserir) => true);
            cmdapagaContacto = new Cmd(apagarContacto, (apaga) => true);
            cmdguardaContacto = new Cmd(guardarContacto, (guarda) => true);
            cmdguardaImagem = new Cmd(guardaImagem, (guardaimg) => true);
            IniciaContacto();
            Fechar = new Cmd(AppSai, (fechar) => true);
        }

        #region Navegar

        public Boolean podeNavegar(object pagina)
        {
            string page = pagina.ToString();
            var currentpage = main.myFrame.Content as Page;
            if (page == currentpage.Title) return false;
            return true;
        }

        public void Navega(Object pagina)
        {
            string page = pagina.ToString();

            switch (page)
            {
                case "pagina1":
                    Page1 p = new Page1();
                    main.myFrame.Navigate(p);
                    break;
                case "pagina2":
                    Page2 p2 = new Page2();
                    main.myFrame.Navigate(p2);
                    break;
            }
        }
        #endregion

        #region Contacto

        public ObservableCollection<Contacto> Contactos
        {
            get { return (ObservableCollection<Contacto>)GetValue(ContactosProperty); }
            set { SetValue(ContactosProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Contactos.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContactosProperty =
            DependencyProperty.Register("Contactos", typeof(ObservableCollection<Contacto>), 
                typeof(MyController), new PropertyMetadata(null));


        public ICollectionView ViewContactos
        {
            get { return (ICollectionView)GetValue(ViewContactosProperty); }
            set { SetValue(ViewContactosProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewContactos.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewContactosProperty =
            DependencyProperty.Register("ViewContactos", typeof(ICollectionView), 
                typeof(MyController), new PropertyMetadata(null));



        public Contacto ContactoActual
        {
            get { return (Contacto)GetValue(ContactoActualProperty); }
            set { SetValue(ContactoActualProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContactoActual.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContactoActualProperty =
            DependencyProperty.Register("ContactoActual", typeof(Contacto), typeof(MyController), new PropertyMetadata(null));



        public void IniciaContacto()
        {
            using (BD_ContactosEntities db = new BD_ContactosEntities())
            {
                List<Contacto> contacto = db.Contactos.ToList();

                Contactos = new ObservableCollection<Contacto>(contacto);
                ViewContactos = CollectionViewSource.GetDefaultView(Contactos);
                ViewContactos.CurrentChanged += ViewContactos_CurrentChanged;
                ContactoActual = (Contacto)ViewContactos.CurrentItem;
            }
        }

        private void ViewContactos_CurrentChanged(object sender, EventArgs e)
        {
            ContactoActual = (Contacto)ViewContactos.CurrentItem;
        }

        #endregion

        #region Botões

        public void inserirContacto(object insere)
        {
            using (BD_ContactosEntities dados = new BD_ContactosEntities())
            {
                
                Contacto novo = new Contacto {Nome="", Telefone="", E_Mail="", Morada="", Imagem=""};
                dados.Contactos.Add(novo);
                dados.SaveChanges();
                IniciaContacto();
                ViewContactos.MoveCurrentToLast();              
            }
        }

        public void guardarContacto(object guarda)
        {
            using (BD_ContactosEntities dados = new BD_ContactosEntities())
            {
                Contacto actual = ViewContactos.CurrentItem as Contacto;
                Contacto edit = dados.Contactos.Find(actual.IdContacto);

                if (edit != null)
                {
                    Page2 pag = main.myFrame.Content as Page2;
                    edit.Nome = pag.txtNome.Text;
                    edit.Telefone = pag.txtTelf.Text;
                    edit.E_Mail = pag.txtEmail.Text;
                    edit.Morada = pag.txtMorada.Text;
                    edit.Imagem = actual.Imagem;
                }
                dados.SaveChanges();
                IniciaContacto();
                ViewContactos.MoveCurrentTo(Contactos.Where(c => c.IdContacto == actual.IdContacto).FirstOrDefault());
            }
        }

        public void apagarContacto(object apaga)
        {
            using (BD_ContactosEntities dados = new BD_ContactosEntities())
            {
                int id;

                if (MessageBox.Show("O Contacto irá ser eliminado!",
                    "Contactos", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    if (int.TryParse(apaga.ToString(), out id))
                    {
                        Contacto del = dados.Contactos.Find(id);
                        if (del != null) dados.Contactos.Remove(del);
                        dados.SaveChanges();
                        IniciaContacto();
                        ViewContactos.MoveCurrentToLast();
                    }
                }
            }
        }

        public void guardaImagem (object imagem)
        {
            try
            {
                System.Windows.Controls.Image img = imagem as System.Windows.Controls.Image;
                if(img != null)
                {
                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.Filter = "Todos |*.*| bmp |*.bmp| jpg |*.jpg| jpeg |*.jpeg| gif |*.gif| png |*.png";

                    if(dlg.ShowDialog() == true)
                    {
                        String caminho = System.Environment.CurrentDirectory;
                        caminho = caminho.Substring(0, caminho.IndexOf("bin"));
                        caminho += @"\Images\";
                        String[] fichs = Directory.GetFiles(caminho);
                        String ficheiro = ContactoActual.IdContacto.ToString() + Path.GetExtension(dlg.FileName);
                        ContactoActual.Imagem = ficheiro;
                        caminho += ficheiro;

                        //if (File.Exists(ficheiro)) File.Delete(caminho);
                        foreach(string f in fichs)
                        {
                            Regex reg = new Regex(ContactoActual.IdContacto.ToString() + @"\.\w*");
                            if (reg.IsMatch(f)) File.Delete(f);
                        }

                        File.Copy(dlg.FileName, caminho, true);

                        img.GetBindingExpression(System.Windows.Controls.Image.SourceProperty).UpdateTarget();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        
        #endregion


        public void AppSai(object fechar)
        {
            if(MessageBox.Show("Deseja fechar a aplicação?",
                    "Contactos", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.main.Close();
            }
        }
    }
}
