using System.Text;

namespace TheOmenDen.EventStore.Infrastructure.Snapshots;

internal sealed class SnapshotStore: ISnapshotStore
{
    private readonly IDbContextFactory<EventStoreContext> _dbContextFactory;

    private readonly string _offlineFilePath;

    public SnapshotStore(IDbContextFactory<EventStoreContext> dbContextFactory)
    {
        var directory = Path.Combine(Directory.GetCurrentDirectory(), "snapshots");

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _offlineFilePath = directory;

        _dbContextFactory = dbContextFactory;
    }

    public void Save(Snapshot snapshot)
    {
        using var context = _dbContextFactory.CreateDbContext();

        var serializedSnapshot = new SerializedSnapshot
        {
            AggregateId = snapshot.AggregateIdentifier,
            AggregateMajorVersion = snapshot.AggregateMajorVersion,
            AggregateMinorVersion = snapshot.AggregateMinorVersion,
            AggregateState = snapshot.AggregateState
        };

        context.SerializedSnapshots.Add(serializedSnapshot);

        context.SaveChanges();
    }

    public async Task SaveAsync(Snapshot snapshot, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var serializedSnapshot = new SerializedSnapshot
        {
            AggregateId = snapshot.AggregateIdentifier,
            AggregateMajorVersion = snapshot.AggregateMajorVersion,
            AggregateMinorVersion = snapshot.AggregateMinorVersion,
            AggregateState = snapshot.AggregateState
        };

        await context.SerializedSnapshots.AddAsync(serializedSnapshot, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public Snapshot Get(Guid id, Type underlyingType)
    {
        using var context = _dbContextFactory.CreateDbContext();

        var materializedSnaphot = context.SerializedSnapshots
            .FirstOrDefault(snapshot => snapshot.Id == id );

        return materializedSnaphot is not null ?  
            new()
        {
            AggregateIdentifier = materializedSnaphot.AggregateId,
            AggregateMajorVersion = materializedSnaphot.AggregateMajorVersion,
            AggregateMinorVersion = materializedSnaphot.AggregateMinorVersion,
            AggregateState = materializedSnaphot.AggregateState
        } : default;

    }
    
    public SerializedSnapshot Get(Guid id)
    {
        using var context = _dbContextFactory.CreateDbContext();

        var serializedSnapshot = context.SerializedSnapshots
            .FirstOrDefault(snapshot => snapshot.Id == id);

        return serializedSnapshot ?? default;
    }

    public async ValueTask<SerializedSnapshot> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var serializedSnapshot = await context.SerializedSnapshots
            .FirstOrDefaultAsync(snapshot => snapshot.Id == id, cancellationToken);
        
        return serializedSnapshot ?? default;
    }

    public async ValueTask<Snapshot> GetAsync(Guid id, Type underlyingType, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var materializedSnaphot = await context.SerializedSnapshots
            .FirstOrDefaultAsync(snapshot => snapshot.Id == id, cancellationToken);

        return materializedSnaphot is not null ?
            new()
            {
                AggregateIdentifier = materializedSnaphot.AggregateId,
                AggregateMajorVersion = materializedSnaphot.AggregateMajorVersion,
                AggregateMinorVersion = materializedSnaphot.AggregateMinorVersion,
                AggregateState = materializedSnaphot.AggregateState
            } : default;
    }

    public void Box(Guid aggregateId)
    {
        var path = Path.Combine(_offlineFilePath, aggregateId.ToString());
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var serializedSnapshot = Get(aggregateId);

        var json = JsonSerializer.Serialize(serializedSnapshot.AggregateState);
        var file = Path.Combine(path, "Snapshot.json");
        File.WriteAllText(file, json, Encoding.Unicode);

        Delete(aggregateId);
    }

    public async Task BoxAsync(Guid aggregateId, CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(_offlineFilePath, aggregateId.ToString());
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var serializedSnapshot = await GetAsync(aggregateId, cancellationToken);

        var file = Path.Combine(path, "Snapshot.json");
        await using var createStream = File.Create(file);
        await JsonSerializer.SerializeAsync(createStream, serializedSnapshot.AggregateState, typeof(AggregateState), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }, cancellationToken);

        await DeleteAsync(aggregateId, cancellationToken);
    }

    public Snapshot Unbox(Guid aggregateId, Type underlyingType)
    {
        // The snapshot must exist!
        var file = Path.Combine(_offlineFilePath, aggregateId.ToString(), "Snapshot.json");
        if (!File.Exists(file))
        {
            throw new SnapshotNotFoundException(file);
        }
        
        // Read the serialized JSON into a new snapshot and return it.
        var json = File.ReadAllText(file, Encoding.Unicode);
        
        return new()
        {
            AggregateIdentifier = aggregateId,
            AggregateMajorVersion = 1,
            AggregateMinorVersion = 0,
            AggregateState = JsonSerializer.Deserialize<AggregateState>(json)
        };
    }

    public async ValueTask<Snapshot> UnboxAsync(Guid aggregateId, Type underlyingType,
        CancellationToken cancellationToken = default)
    {
        var file = Path.Combine(_offlineFilePath, aggregateId.ToString(), "Snapshot.json");
        
        if (!File.Exists(file))
        {
            throw new SnapshotNotFoundException(file);
        }

        await using var fileStream = File.OpenRead(file);

        var snapshot = await JsonSerializer.DeserializeAsync<Snapshot>(fileStream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }, cancellationToken);

        await DeleteAsync(aggregateId, cancellationToken);

        return snapshot ?? default;
    }

    private void Delete(Guid aggregateId)
    {
        using var context = _dbContextFactory.CreateDbContext();

        var snapshotToRemove = context.SerializedSnapshots.FirstOrDefault(snapshot => snapshot.Id == aggregateId);

        if (snapshotToRemove is null)
        {
            return;
        }

        context.SerializedSnapshots.Remove(snapshotToRemove);

        context.SaveChanges();
    }

    private async Task DeleteAsync(Guid aggregateId, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var snapshotToRemove = await context.SerializedSnapshots.FirstOrDefaultAsync(snapshot => snapshot.Id == aggregateId, cancellationToken);

        if (snapshotToRemove is null)
        {
            return;
        }

        context.SerializedSnapshots.Remove(snapshotToRemove);

        await context.SaveChangesAsync(cancellationToken);
    }
}