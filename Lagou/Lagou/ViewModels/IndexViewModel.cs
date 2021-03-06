﻿using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using Lagou.API;
using Lagou.API.Entities;
using Lagou.API.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Lagou.ViewModels {

    [Regist(InstanceMode.Singleton)]
    public class IndexViewModel : BaseVM {

        public override string Title {
            get {
                return "职位列表";
            }
        }

        private SimpleContainer Container = null;
        private INavigationService NS = null;

        public BindableCollection<SearchedItemViewModel> Datas { get; set; }

        public ICommand ReloadCmd { get; set; }

        public ICommand LoadMoreCmd { get; set; }

        private int Page = 1;

        public IndexViewModel(SimpleContainer container, INavigationService ns) {
            this.Datas = new BindableCollection<SearchedItemViewModel>();
            //this.Datas.CollectionChanged += Datas_CollectionChanged;
            this.Container = container;
            this.NS = ns;

            this.ReloadCmd = new Command(async () => {
                await this.LoadData(true);
            });

            this.LoadMoreCmd = new Command(async () => {
                await this.LoadData(false);
            });
        }

        //private void Datas_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
        //    var act = e.Action;
        //}

        protected async override void OnActivate() {
            base.OnActivate();
            await Task.Delay(500).ContinueWith(t => this.LoadData());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reload">是否是重新加载</param>
        /// <returns></returns>
        private async Task LoadData(bool reload = false) {
            this.IsBusy = true;

            var method = new Search() {
                Page = reload ? 1 : this.Page
            };
            var datas = await ApiClient.Execute(method);
            if (!method.HasError && datas.Count() > 0) {

                if (reload) {
                    this.Datas.Clear();
                }


                if (Device.OS == TargetPlatform.Windows) {
                    foreach (var d in datas) {
                        this.Datas.Add(new SearchedItemViewModel(d, this.NS));
                    }
                } else
                    this.Datas.AddRange(datas.Select(d =>
                        new SearchedItemViewModel(d, this.NS)
                    ));

                //this.NotifyOfPropertyChange(() => this.Datas);
                this.Page++;
            }

            this.IsBusy = false;
        }
    }
}
