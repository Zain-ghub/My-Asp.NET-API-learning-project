using Microsoft.AspNetCore.SignalR;

namespace RepoApi.Hubs
{
    public class StockHub : Hub
    {
        public async Task BroadcastStockUpdate(string productName, int newStock) { 
        
            await Clients.All.SendAsync("StockUpdated", productName, newStock);
        }
    }
}
