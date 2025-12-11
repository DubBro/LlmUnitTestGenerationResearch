using AutoMapper;

namespace Dataset.Sample6;

public class LotService : Service, ILotService
{
    public LotService(IUnitOfWork database, IMapper mapper)
        : base(database, mapper)
    {
    }

    public void AddLot(LotDTO lot)
    {
        if (lot == null || lot.Sold == true || lot.Name == null || lot.Owner == null || lot.Category == null)
        {
            throw new InvalidLotException();
        }

        lot.Auction = new AuctionDTO();

        _database.Lots.Add(_mapper.Map<LotDTO, Lot>(lot));
        _database.Commit();
    }

    public void DeleteLot(int id)
    {
        if (id <= 0)
        {
            throw new InvalidIdException();
        }

        _database.Auctions.Delete(id);
        _database.Lots.Delete(id);
        _database.Commit();
    }

    public LotDTO GetLot(int id)
    {
        if (id <= 0)
        {
            throw new InvalidIdException();
        }

        var lot = _mapper.Map<Lot, LotDTO>(_database.Lots.Get(id))
            ?? throw new InvalidIdException();

        return lot;
    }

    public IEnumerable<LotDTO> GetLots()
    {
        return _mapper.Map<IEnumerable<Lot>, IEnumerable<LotDTO>>(_database.Lots.GetAll());
    }

    public IEnumerable<LotDTO> GetLotsByCategory(string category)
    {
        if (category == null)
        {
            throw new InvalidCategoryException();
        }

        return _mapper.Map<IEnumerable<Lot>, IEnumerable<LotDTO>>(_database.Lots.GetLotsByCategory(category));
    }

    public IEnumerable<LotDTO> GetLotsByName(string name)
    {
        if (name == null)
        {
            throw new InvalidNameException();
        }

        return _mapper.Map<IEnumerable<Lot>, IEnumerable<LotDTO>>(_database.Lots.GetLotsByName(name));
    }

    public IEnumerable<LotDTO> GetNotSoldLots()
    {
        return _mapper.Map<IEnumerable<Lot>, IEnumerable<LotDTO>>(_database.Lots.GetNotSoldLots());
    }

    public IEnumerable<LotDTO> GetSoldLots()
    {
        return _mapper.Map<IEnumerable<Lot>, IEnumerable<LotDTO>>(_database.Lots.GetSoldLots());
    }

    public void UpdateLot(LotDTO lotDto)
    {
        if (lotDto == null || lotDto.Name == null || lotDto.Owner == null || lotDto.Category == null)
        {
            throw new InvalidLotException();
        }

        var lot = _database.Lots.Get(lotDto.ID);

        if (lot == null)
        {
            throw new InvalidIdException();
        }

        lot.Name = lotDto.Name;
        lot.Owner = lotDto.Owner;
        lot.Sold = lotDto.Sold;
        lot.Category = lotDto.Category;
        lot.Details = lotDto.Details;

        _database.Lots.Update(lot);
        _database.Commit();
    }
}

public abstract class Service
{
    protected readonly IUnitOfWork _database;
    protected readonly IMapper _mapper;

    protected Service(IUnitOfWork database, IMapper mapper)
    {
        _database = database;
        _mapper = mapper;
    }
}

public interface ILotService
{
    void AddLot(LotDTO lot);
    LotDTO GetLot(int id);
    IEnumerable<LotDTO> GetLots();
    IEnumerable<LotDTO> GetLotsByCategory(string category);
    IEnumerable<LotDTO> GetLotsByName(string name);
    IEnumerable<LotDTO> GetSoldLots();
    IEnumerable<LotDTO> GetNotSoldLots();
    void UpdateLot(LotDTO lotDto);
    void DeleteLot(int id);
}

public interface IUnitOfWork : IDisposable
{
    ILotRepository Lots { get; }
    IAuctionRepository Auctions { get; }

    void Commit();
}

public interface ILotRepository : IRepository<Lot>
{
    IEnumerable<Lot> GetLotsByCategory(string category);
    IEnumerable<Lot> GetLotsByName(string name);
    IEnumerable<Lot> GetSoldLots();
    IEnumerable<Lot> GetNotSoldLots();
}

public interface IAuctionRepository : IRepository<Auction>
{
}

public interface IRepository<TEntity> where TEntity : class
{
    void Add(TEntity entity);
    TEntity Get(int id);
    IEnumerable<TEntity> GetAll();
    void Update(TEntity entity);
    void Delete(int id);
}

public class LotDTO
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Details { get; set; }
    public string Owner { get; set; }
    public bool Sold { get; set; }
    public string Category { get; set; }

    public AuctionDTO Auction { get; set; }
}

public class AuctionDTO
{
    public int ID { get; set; }
    public int Bid { get; set; }
    public string Leader { get; set; }
    public bool Started { get; set; }
    public bool Ended { get; set; }

    public LotDTO Lot { get; set; }
}

public class Lot
{
    public virtual int ID { get; set; }
    public virtual string Name { get; set; }
    public virtual string Details { get; set; }
    public virtual string Owner { get; set; }
    public virtual bool Sold { get; set; }
    public virtual string Category { get; set; }

    public virtual Auction Auction { get; set; }
}

public class Auction
{
    public virtual int ID { get; set; }
    public virtual int Bid { get; set; }
    public virtual string Leader { get; set; }
    public virtual bool Started { get; set; }
    public virtual bool Ended { get; set; }

    public virtual Lot Lot { get; set; }
}

public class InvalidIdException : Exception
{
    public InvalidIdException(string message = "ERROR: Invalid ID")
        : base(message)
    {
    }
}

public class InvalidLotException : Exception
{
    public InvalidLotException(string message = "ERROR: Invalid lot")
        : base(message)
    {
    }
}

public class InvalidCategoryException : Exception
{
    public InvalidCategoryException(string message = "ERROR: Invalid category")
        : base(message)
    {
    }
}

public class InvalidNameException : Exception
{
    public InvalidNameException(string message = "ERROR: Invalid name")
        : base(message)
    {
    }
}
