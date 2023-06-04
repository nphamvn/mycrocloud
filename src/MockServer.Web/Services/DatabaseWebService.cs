using System.Text;
using AutoMapper;
using MockServer.Core.Databases;
using MockServer.Core.Repositories;
using MockServer.Core.Services;
using MockServer.Web.Models.Database;
using MockServer.Web.Services.Interfaces;

namespace MockServer.Web.Services;

public class DatabaseWebService : BaseService, IDatabaseWebService
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

    public async Task ConfigureApplication(int id, Service service, bool allowed)
    {
        var db = await _databaseRepository.Get(id);
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

    public Task Delete(int id)
    {
        return _databaseRepository.Delete(id);
    }

    public Task Edit(int id, SaveDatabaseViewModel vm)
    {
        var db = _mapper.Map<Database>(vm);
        return _databaseRepository.Update(id, db);
    }

    public async Task<string> GetData(int id)
    {
        var db = await _databaseRepository.Get(id);
        return db.Data;
    }

    public async Task<byte[]> GetDataBinary(int id)
    {
        var db = await _databaseRepository.Get(id);
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
        var vm = _mapper.Map<SaveDatabaseViewModel>(db);
        var applications = await _databaseRepository.GetDatabaseUsingService(id);
        vm.AllServices = (await _serviceRepository.GetServices(AuthUser.Id))
                            .Where(s => s.Type != ServiceType.Database);
        vm.AllowedService = _mapper.Map<IEnumerable<Service>>(applications);
        return vm;
    }

    public Task SaveData(int id, string data)
    {
        return _databaseRepository.UpdateData(id, data);
    }
}
