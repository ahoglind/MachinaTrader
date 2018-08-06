using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Mynt.Core.Interfaces;
using Mynt.Core.Models;

namespace Mynt.Data.LiteDB
{

    public class LiteDbDataStore : IDataStore
    {
        private readonly LiteDatabase _database;
        private readonly LiteCollection<TraderAdapter> _traderAdapter;
        private readonly LiteCollection<TradeAdapter> _ordersAdapter;

        public LiteDbDataStore(LiteDbOptions options)
        {
            _database = new LiteDatabase(options.LiteDbName);
            _ordersAdapter = _database.GetCollection<TradeAdapter>("Orders");
            _traderAdapter = _database.GetCollection<TraderAdapter>("Traders");
        }

        public async Task InitializeAsync()
        {
        }

        public async Task<List<Trade>> GetClosedTradesAsync()
        {
            var trades = _ordersAdapter.Find(x => !x.IsOpen).ToList();
            var items = Mapping.Mapper.Map<List<Trade>>(trades);

            return items;
        }

        public async Task<List<Trade>> GetActiveTradesAsync()
        {
            var trades = _ordersAdapter.Find(x => x.IsOpen).ToList();
            var items = Mapping.Mapper.Map<List<Trade>>(trades);

            return items;
        }

        public async Task<List<Trader>> GetAvailableTradersAsync()
        {
            var traders = _traderAdapter.Find(x => !x.IsBusy && !x.IsArchived).ToList();
            var items = Mapping.Mapper.Map<List<Trader>>(traders);

            return items;
        }

        public async Task<List<Trader>> GetBusyTradersAsync()
        {
            var traders = _traderAdapter.Find(x => x.IsBusy && !x.IsArchived).ToList();
            var items = Mapping.Mapper.Map<List<Trader>>(traders);

            return items;
        }

        public async Task SaveTradeAsync(Trade trade)
        {
            var item = Mapping.Mapper.Map<TradeAdapter>(trade);
            TradeAdapter checkExist = _ordersAdapter.Find(x => x.TradeId.Equals(item.TradeId)).FirstOrDefault();
            _ordersAdapter.Upsert(item);
        }

        public async Task SaveTraderAsync(Trader trader)
        {
            var item = Mapping.Mapper.Map<TraderAdapter>(trader);
            TraderAdapter checkExist = _traderAdapter.Find(x => x.Identifier.Equals(item.Identifier)).FirstOrDefault();
            _traderAdapter.Upsert(item);
        }

        public async Task SaveTradersAsync(List<Trader> traders)
        {
            var items = Mapping.Mapper.Map<List<TraderAdapter>>(traders);

            foreach (var item in items)
            {
                TraderAdapter checkExist = _traderAdapter.Find(x => x.Identifier.Equals(item.Identifier)).FirstOrDefault();
                _traderAdapter.Upsert(item);
            }
        }

        public async Task SaveTradesAsync(List<Trade> trades)
        {
            var items = Mapping.Mapper.Map<List<TradeAdapter>>(trades);

            foreach (var item in items)
            {
                TradeAdapter checkExist = _ordersAdapter.Find(x => x.TradeId.Equals(item.TradeId)).FirstOrDefault();
                _ordersAdapter.Upsert(item);
            }
        }

        public async Task<List<Trader>> GetTradersAsync()
        {
            var traders = _traderAdapter.FindAll().ToList();
            var items = Mapping.Mapper.Map<List<Trader>>(traders);

            return items;
        }
    }
}
