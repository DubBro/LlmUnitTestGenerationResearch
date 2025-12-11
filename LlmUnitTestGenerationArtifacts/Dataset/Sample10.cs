using AutoMapper;

namespace Dataset.Sample10;

public class AuctionService : IAuctionService
{
    private readonly IUnitOfWork _database;
    private readonly IMapper _mapper;

    public AuctionService(IUnitOfWork database, IMapper mapper)
    {
        _database = database;
        _mapper = mapper;
    }

    public void Bet(int id, string customerName, int bid)
    {
        if (id <= 0)
        {
            throw new InvalidIdException();
        }

        if (customerName == null)
        {
            throw new InvalidNameException();
        }

        var auction = _database.Auctions.Get(id);

        if (auction == null)
        {
            throw new InvalidIdException();
        }

        if (!auction.Started)
        {
            throw new InvalidAuctionException("ERROR: Auction has not started yet");
        }

        if (auction.Ended)
        {
            throw new InvalidAuctionException("ERROR: Auction is over");
        }

        if (auction.Bid >= bid)
        {
            throw new InvalidAuctionException("ERROR: Invalid bid");
        }

        auction.Bid = bid;
        auction.Leader = customerName;

        _database.Auctions.Update(auction);
        _database.Commit();
    }

    public void CloseAuction(int id)
    {
        if (id <= 0)
        {
            throw new InvalidIdException();
        }

        var auction = _database.Auctions.Get(id);

        if (auction == null)
        {
            throw new InvalidIdException();
        }

        if (!auction.Started)
        {
            throw new InvalidAuctionException("ERROR: Auction has not started yet");
        }

        if (auction.Ended)
        {
            throw new InvalidAuctionException("ERROR: Auction has already finished");
        }

        auction.Ended = true;

        _database.Auctions.Update(auction);
        _database.Commit();
    }

    public AuctionDTO GetAuction(int id)
    {
        if (id <= 0)
        {
            throw new InvalidIdException();
        }

        var auction = _mapper.Map<Auction, AuctionDTO>(_database.Auctions.Get(id))
            ?? throw new InvalidIdException();

        return auction;
    }

    public IEnumerable<AuctionDTO> GetAuctions()
    {
        return _mapper.Map<IEnumerable<Auction>, IEnumerable<AuctionDTO>>(_database.Auctions.GetAll());
    }

    public void OpenAuction(int id)
    {
        if (id <= 0)
        {
            throw new InvalidIdException();
        }

        var auction = _database.Auctions.Get(id);

        if (auction == null)
        {
            throw new InvalidIdException();
        }

        if (auction.Ended)
        {
            throw new InvalidAuctionException("ERROR: Auction has already finished");
        }

        if (auction.Started)
        {
            throw new InvalidAuctionException("ERROR: Auction has already started");
        }

        auction.Started = true;

        _database.Auctions.Update(auction);
        _database.Commit();
    }
}

public interface IAuctionService
{
    AuctionDTO GetAuction(int id);
    IEnumerable<AuctionDTO> GetAuctions();
    void OpenAuction(int id);
    void CloseAuction(int id);
    void Bet(int id, string customerName, int bid);
}

public interface IUnitOfWork : IDisposable
{
    IAuctionRepository Auctions { get; }

    void Commit();
}

public interface IAuctionRepository
{
    Auction Get(int id);
    IEnumerable<Auction> GetAll();
    void Update(Auction entity);
}

public class AuctionDTO
{
    public int ID { get; set; }
    public int Bid { get; set; }
    public string Leader { get; set; }
    public bool Started { get; set; }
    public bool Ended { get; set; }
}

public class Auction
{
    public virtual int ID { get; set; }
    public virtual int Bid { get; set; }
    public virtual string Leader { get; set; }
    public virtual bool Started { get; set; }
    public virtual bool Ended { get; set; }
}

public class InvalidAuctionException : Exception
{
    public InvalidAuctionException()
        : base()
    {
    }

    public InvalidAuctionException(string message = "ERROR: Invalid auction")
        : base(message)
    {
    }

    public InvalidAuctionException(string message = "ERROR: Invalid auction", Exception? innerException = null)
        : base(message,  innerException)
    {
    }
}

public class InvalidIdException : Exception
{
    public InvalidIdException()
        : base()
    {
    }

    public InvalidIdException(string message = "ERROR: Invalid ID")
        : base(message)
    {
    }

    public InvalidIdException(string message = "ERROR: Invalid ID", Exception? innerException = null)
        : base(message, innerException)
    {
    }
}

public class InvalidNameException : Exception
{
    public InvalidNameException()
        : base()
    {
    }

    public InvalidNameException(string message = "ERROR: Invalid name")
        : base(message)
    {
    }

    public InvalidNameException(string message = "ERROR: Invalid name", Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
