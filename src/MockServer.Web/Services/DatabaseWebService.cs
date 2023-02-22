using System.Text;
using AutoMapper;
using MockServer.Core.Databases;
using MockServer.Core.Enums;
using MockServer.Core.Models.Services;
using MockServer.Core.Repositories;
using MockServer.Web.Models.Database;
using MockServer.Web.Services.Interfaces;

namespace MockServer.Web.Services;

public class DatabaseWebService : BaseWebService, IDatabaseWebService
{
    private readonly IDatabaseRepository _databaseRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IMapper _mapper;

    public DatabaseWebService(IDatabaseRepository databaseRepository,
            IServiceRepository serviceRepository, 
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
    {
        _databaseRepository = databaseRepository;
        _serviceRepository = serviceRepository;
        _mapper = mapper;
    }

    public async Task ConfigureApplication(string name, Service service, bool allowed)
    {
        var db = await _databaseRepository.Find(AuthUser.Id, name);
        var services = await _databaseRepository.GetDatabaseUsingService(db.Id);
        List<Service> currentList;
        if (services != null)
        {
            currentList = services.ToList();
        }
        else 
        {
            currentList = new();
        }
        if (allowed)
        {
            if (!currentList.Any(s => s.Type == service.Type && s.Id == service.Id))
            {
                currentList.Add(new()
                {
                    Id = service.Id,
                    Type = service.Type
                });
            }
            else 
            {
                return;
            }
        }
        else 
        {
            var item = currentList.SingleOrDefault(s => s.Type == service.Type && s.Id == service.Id);
            if (item != null)
            {
                currentList.Remove(item);
            }
            else
            {
                return;
            }
        }
        await _databaseRepository.UpdateDatabaseUsingService(db.Id, currentList);
    }

    public Task Create(SaveDatabaseViewModel vm)
    {
        var db = _mapper.Map<Database>(vm);
        return _databaseRepository.Add(AuthUser.Id, db);
    }

    public Task Edit(int id, SaveDatabaseViewModel vm)
    {
        var db = _mapper.Map<Database>(vm);
        return _databaseRepository.Update(AuthUser.Id, db);
    }

    public async Task<byte[]> GetDataBinary(string name)
    {
        var db = await _databaseRepository.Find(AuthUser.Id, name);
        return Encoding.UTF8.GetBytes(db.Data);
    }

    public async Task<IndexViewModel> GetIndexViewModel()
    {
        var vm = new IndexViewModel();
        var dbs = await _databaseRepository.GetAll(AuthUser.Id);
        vm.DatabaseItems = _mapper.Map<IEnumerable<DatabaseItem>>(dbs);
        return vm;
    }

    public async Task<SaveDatabaseViewModel> GetViewModel(int id)
    {
        var db = await _databaseRepository.Get(id);
        return _mapper.Map<SaveDatabaseViewModel>(db);
    }

    public async Task<SaveDatabaseViewModel> GetViewModel(string name)
    {
        var db = await _databaseRepository.Find(AuthUser.Id, name);
        var vm =  _mapper.Map<SaveDatabaseViewModel>(db);
        var applications = await _databaseRepository.GetDatabaseUsingService(db.Id);
        vm.AllServices = (await _serviceRepository.GetServices(AuthUser.Id))
                            .Where(s => s.Type != ServiceType.Database);
        vm.AllowedService = _mapper.Map<IEnumerable<Service>>(applications);
        return vm;
    }
}
