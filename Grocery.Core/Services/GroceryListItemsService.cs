using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class GroceryListItemsService : IGroceryListItemsService
    {
        private readonly IGroceryListItemsRepository _groceriesRepository;
        private readonly IProductRepository _productRepository;

        public GroceryListItemsService(IGroceryListItemsRepository groceriesRepository, IProductRepository productRepository)
        {
            _groceriesRepository = groceriesRepository;
            _productRepository = productRepository;
        }

        public List<GroceryListItem> GetAll()
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll().Where(g => g.GroceryListId == groceryListId).ToList();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            return _groceriesRepository.Add(item);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        public GroceryListItem? Get(int id)
        {
            return _groceriesRepository.Get(id);
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            return _groceriesRepository.Update(item);
        }

        public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)
        {
            // Haal alle boodschappenlijstitems op
            var allItems = _groceriesRepository.GetAll();

            // Groepeer op ProductId en tel het aantal verkopen
            var grouped = allItems
                .GroupBy(item => item.ProductId)
                .Select(g => new {
                    ProductId = g.Key,
                    NrOfSells = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.NrOfSells)
                .Take(topX)
                .ToList();

            // Maak een lijst van BestSellingProducts
            var result = new List<BestSellingProducts>();
            int ranking = 1;
            foreach (var group in grouped)
            {
                var product = _productRepository.Get(group.ProductId);
                if (product != null)
                {
                    result.Add(new BestSellingProducts(
                        product.Id,
                        product.Name,
                        product.Stock,
                        group.NrOfSells,
                        ranking
                    ));
                    ranking++;
                }
            }
            return result;
        }

        private void FillService(List<GroceryListItem> groceryListItems)
        {
            foreach (GroceryListItem g in groceryListItems)
            {
                g.Product = _productRepository.Get(g.ProductId) ?? new(0, "", 0);
            }
        }
    }
}
