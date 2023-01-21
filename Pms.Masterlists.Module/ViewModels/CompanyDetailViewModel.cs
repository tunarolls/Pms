using Pms.Common;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pms.Masterlists.Module.ViewModels
{
    public class CompanyDetailViewModel : BindableBase
    {
        private IEnumerable<Company> _companies;
        private Company selectedCompany;
        private readonly Companies m_Companies;

        public CompanyDetailViewModel(Companies m_Companies)
        {
            this.m_Companies = m_Companies;

            //SelectedCompany = new();

            //SaveCommand = new Save(this, companiesM);
            //ListingCommand = new Listing(this, companiesM);
            //ListingCommand.Execute(null);

            SaveCommand = new DelegateCommand(Save);
            ListingCommand = new DelegateCommand(Listing);
        }

        public IEnumerable<Company> Companies { get => _companies; set => SetProperty(ref _companies, value); }
        
        public Company SelectedCompany { get => selectedCompany; set => SetProperty(ref selectedCompany, value); }

        //public ObservableCollection<string> Sites
        //{
        //    get
        //    {
        //        List<string> sites = new();
        //        foreach (SiteChoices site in Enum.GetValues(typeof(SiteChoices)))
        //            sites.Add(site.ToString());

        //        return new ObservableCollection<string>(sites);
        //    }
        //}

        #region commands
        public DelegateCommand ListingCommand { get; }
        public DelegateCommand SaveCommand { get; }

        private void Listing()
        {
            _ = Listing(default);
        }

        private void Save()
        {
            _ = Save(default);
        }
        #endregion

        private async Task Listing(CancellationToken cancellationToken = default)
        {
            try
            {
                Companies = await m_Companies.ListCompanies(cancellationToken);
            }
            catch
            {
                throw;
            }
        }

        private async Task Save(CancellationToken cancellationToken = default)
        {
            try
            {
                var company = SelectedCompany;
                company.CompanyId = Company.GenerateId(company);
                await m_Companies.Save(company, cancellationToken);
            }
            catch
            {
                throw;
            }
        }
    }
}
