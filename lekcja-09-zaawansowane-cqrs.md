# Lekcja 9: Zaawansowane CQRS – Paginacja, Filtrowanie, Audyt

**Moduł:** Optymalizacja zapytań, śledzenie zmian  
**Czas trwania:** 2,5 godziny  
**Poziom:** Zaawansowany

---

## 🎯 Cele Lekcji

Po ukończeniu tej lekcji będziesz potrafić:
- ✅ Implementować paginację w API i React Native
- ✅ Budować zaawansowane filtrowanie i sortowanie
- ✅ Dodawać pola audytowe (CreatedAt, UpdatedAt, CreatedBy)
- ✅ Tworzyć bazową encję z polami audytowymi
- ✅ Implementować automatyczne śledzenie zmian
- ✅ Konfigurować logowanie do pliku (Serilog)
- ✅ Optymalizować zapytania (Include, Select, AsNoTracking)

---

## CZĘŚĆ 1: Teoria Paginacji i Filtrowania (20 minut)

### 1.1. Dlaczego paginacja jest kluczowa?

**SCRIPT dla prowadzącego:**

> „Wyobraźcie sobie sklep z 100 000 produktów. Jeśli pobierzemy wszystkie naraz, aplikacja się zawiesi. Paginacja to podział na strony - pobieramy np. 20 rekordów na raz."

**Problemy bez paginacji:**

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                    BEZ PAGINACJI - PROBLEMY                                  │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│   📱 React Native                     🖥️ API                                │
│   ┌─────────────────┐                ┌─────────────────┐                    │
│   │ GET /api/items  │───────────────▶│ SELECT * FROM   │                    │
│   │                 │                │ Items           │                    │
│   └────────┬────────┘                └────────┬────────┘                    │
│            │                                   │                            │
│            │                                   ▼                            │
│            │                         ┌─────────────────┐                    │
│            │                         │ 100 000 rekordów │                   │
│            │                         │ = 50 MB JSON!    │                   │
│            │                         └────────┬────────┘                    │
│            │                                   │                            │
│            │◀───────────────────────────────────┘                           │
│            │                                                                 │
│            ▼                                                                 │
│   ┌─────────────────┐                                                        │
│   │ ❌ PROBLEMY:     │                                                       │
│   │ • OutOfMemory   │                                                        │
│   │ • 10s load time │                                                        │
│   │ • Zużycie RAM   │                                                        │
│   │ • Frozen UI     │                                                        │
│   └─────────────────┘                                                        │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

**Z paginacją:**

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                    Z PAGINACJĄ - ROZWIĄZANIE                                 │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│   📱 React Native                     🖥️ API                                │
│   ┌─────────────────┐                ┌─────────────────┐                    │
│   │ GET /api/items  │───────────────▶│ SELECT TOP 20   │                    │
│   │ ?page=1         │                │ FROM Items      │                    │
│   │ &pageSize=20    │                │ OFFSET 0        │                    │
│   └────────┬────────┘                └────────┬────────┘                    │
│            │                                   │                            │
│            │                                   ▼                            │
│            │                         ┌─────────────────┐                    │
│            │                         │ 20 rekordów     │                    │
│            │                         │ + metadata      │                    │
│            │                         │ = 10 KB JSON    │                    │
│            │                         └────────┬────────┘                    │
│            │                                   │                            │
│            │◀───────────────────────────────────┘                           │
│            │                                                                 │
│            ▼                                                                 │
│   ┌─────────────────┐                                                        │
│   │ ✅ ZALETY:       │                                                       │
│   │ • Szybkie       │                                                        │
│   │ • Mało pamięci  │                                                        │
│   │ • Płynne UI     │                                                        │
│   │ • Infinite load │                                                        │
│   └─────────────────┘                                                        │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 1.2. Rodzaje paginacji

| Typ | Opis | Zalety | Wady |
|-----|------|--------|------|
| **Offset** | `SKIP N TAKE M` | Proste, popularne | Problemy przy zmianach danych |
| **Keyset** | `WHERE Id > lastId` | Szybkie, stabilne | Trudniejsze w implementacji |
| **Cursor** | Token do następnej strony | Elastyczne | Wymaga dodatkowej logiki |

**W tej lekcji używamy Offset pagination (najprostsze i najczęstsze).**

### 1.3. Struktura odpowiedzi paginowanej

```json
{
  "items": [...],           // Dane na aktualnej stronie
  "pageNumber": 1,          // Numer aktualnej strony
  "pageSize": 20,           // Rozmiar strony
  "totalCount": 1543,       // Całkowita liczba rekordów
  "totalPages": 78,         // Całkowita liczba stron
  "hasPreviousPage": false, // Czy jest poprzednia strona
  "hasNextPage": true       // Czy jest następna strona
}
```

---

## CZĘŚĆ 2: Implementacja Paginacji (.NET) (35 minut)

### 2.1. Klasa PaginatedList

**Common/PaginatedList.cs:**

```csharp
using Microsoft.EntityFrameworkCore;

namespace SolutionOrdersReact.Server.Common
{
    /// <summary>
    /// Generyczna lista paginowana
    /// </summary>
    /// <typeparam name="T">Typ elementów</typeparam>
    public class PaginatedList<T>
    {
        /// <summary>
        /// Elementy na aktualnej stronie
        /// </summary>
        public List<T> Items { get; set; } = new();

        /// <summary>
        /// Numer aktualnej strony (1-indexed)
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Liczba elementów na stronie
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Całkowita liczba elementów
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Całkowita liczba stron
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        /// <summary>
        /// Czy istnieje poprzednia strona
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Czy istnieje następna strona
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = count;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        /// <summary>
        /// Tworzy paginowaną listę z IQueryable
        /// </summary>
        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            // Najpierw policz wszystkie rekordy
            var count = await source.CountAsync(cancellationToken);

            // Pobierz tylko wymaganą stronę
            var items = await source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
```

### 2.2. Parametry żądania paginowanego

**Common/PaginatedRequest.cs:**

```csharp
namespace SolutionOrdersReact.Server.Common
{
    /// <summary>
    /// Bazowe parametry dla żądań paginowanych
    /// </summary>
    public class PaginatedRequest
    {
        private int _pageNumber = 1;
        private int _pageSize = 20;

        /// <summary>
        /// Numer strony (min 1)
        /// </summary>
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        /// <summary>
        /// Rozmiar strony (min 1, max 100)
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? 1 : (value > 100 ? 100 : value);
        }

        /// <summary>
        /// Pole do sortowania
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Kierunek sortowania (asc/desc)
        /// </summary>
        public string SortDirection { get; set; } = "asc";

        /// <summary>
        /// Czy sortowanie malejące
        /// </summary>
        public bool IsDescending => SortDirection.ToLower() == "desc";
    }
}
```

### 2.3. GetAllItemsPaginatedQuery

**Features/Items/Queries/GetAllItemsPaginated/GetAllItemsPaginatedQuery.cs:**

```csharp
using MediatR;
using SolutionOrdersReact.Server.Common;
using SolutionOrdersReact.Server.Dto;

namespace SolutionOrdersReact.Server.Features.Items.Queries.GetAllItemsPaginated
{
    /// <summary>
    /// Query do pobierania paginowanej listy produktów
    /// </summary>
    public class GetAllItemsPaginatedQuery
        : PaginatedRequest, IRequest<PaginatedList<ItemDto>>
    {
        // ========== FILTRY ==========

        /// <summary>
        /// Szukaj po nazwie/kodzie/opisie
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Filtruj po kategorii
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// Filtruj po jednostce miary
        /// </summary>
        public int? UnitOfMeasurementId { get; set; }

        /// <summary>
        /// Minimalna cena
        /// </summary>
        public decimal? MinPrice { get; set; }

        /// <summary>
        /// Maksymalna cena
        /// </summary>
        public decimal? MaxPrice { get; set; }

        /// <summary>
        /// Tylko aktywne (domyślnie true)
        /// </summary>
        public bool? IsActive { get; set; } = true;

        /// <summary>
        /// Tylko z dostępnym stanem
        /// </summary>
        public bool? InStock { get; set; }
    }
}
```

### 2.4. GetAllItemsPaginatedHandler

**Features/Items/Queries/GetAllItemsPaginated/GetAllItemsPaginatedHandler.cs:**

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Common;
using SolutionOrdersReact.Server.Data;
using SolutionOrdersReact.Server.Dto;
using System.Linq.Expressions;

namespace SolutionOrdersReact.Server.Features.Items.Queries.GetAllItemsPaginated
{
    public class GetAllItemsPaginatedHandler
        : IRequestHandler<GetAllItemsPaginatedQuery, PaginatedList<ItemDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetAllItemsPaginatedHandler> _logger;

        public GetAllItemsPaginatedHandler(
            ApplicationDbContext context,
            ILogger<GetAllItemsPaginatedHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaginatedList<ItemDto>> Handle(
            GetAllItemsPaginatedQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Pobieranie produktów: strona {Page}, rozmiar {Size}, szukaj: {Search}",
                request.PageNumber, request.PageSize, request.Search);

            // Bazowe query z Include
            var query = _context.Items
                .Include(i => i.Category)
                .Include(i => i.UnitOfMeasurement)
                .AsNoTracking()  // Optymalizacja - nie śledzimy zmian
                .AsQueryable();

            // ========== FILTROWANIE ==========

            // Filtr aktywności
            if (request.IsActive.HasValue)
            {
                query = query.Where(i => i.IsActive == request.IsActive.Value);
            }

            // Wyszukiwanie tekstowe
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchLower = request.Search.ToLower();
                query = query.Where(i =>
                    (i.Name != null && i.Name.ToLower().Contains(searchLower)) ||
                    (i.Code != null && i.Code.ToLower().Contains(searchLower)) ||
                    (i.Description != null && i.Description.ToLower().Contains(searchLower))
                );
            }

            // Filtr kategorii
            if (request.CategoryId.HasValue)
            {
                query = query.Where(i => i.IdCategory == request.CategoryId.Value);
            }

            // Filtr jednostki miary
            if (request.UnitOfMeasurementId.HasValue)
            {
                query = query.Where(i => i.IdUnitOfMeasurement == request.UnitOfMeasurementId.Value);
            }

            // Filtr ceny
            if (request.MinPrice.HasValue)
            {
                query = query.Where(i => i.Price >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                query = query.Where(i => i.Price <= request.MaxPrice.Value);
            }

            // Filtr dostępności
            if (request.InStock == true)
            {
                query = query.Where(i => i.Quantity > 0);
            }

            // ========== SORTOWANIE ==========

            query = ApplySorting(query, request.SortBy, request.IsDescending);

            // ========== PROJEKCJA DO DTO ==========

            var dtoQuery = query.Select(i => new ItemDto
            {
                IdItem = i.IdItem,
                Name = i.Name,
                Code = i.Code,
                Description = i.Description,
                Price = i.Price,
                Quantity = i.Quantity,
                IsActive = i.IsActive,
                IdCategory = i.IdCategory,
                CategoryName = i.Category != null ? i.Category.Name : null,
                IdUnitOfMeasurement = i.IdUnitOfMeasurement,
                UnitName = i.UnitOfMeasurement != null ? i.UnitOfMeasurement.Name : null
            });

            // ========== PAGINACJA ==========

            var result = await PaginatedList<ItemDto>.CreateAsync(
                dtoQuery,
                request.PageNumber,
                request.PageSize,
                cancellationToken
            );

            _logger.LogInformation(
                "Znaleziono {TotalCount} produktów, zwracam stronę {PageNumber}/{TotalPages}",
                result.TotalCount, result.PageNumber, result.TotalPages);

            return result;
        }

        /// <summary>
        /// Aplikuje sortowanie dynamicznie
        /// </summary>
        private IQueryable<Models.Item> ApplySorting(
            IQueryable<Models.Item> query,
            string? sortBy,
            bool descending)
        {
            // Domyślne sortowanie
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return query.OrderBy(i => i.Name);
            }

            // Mapa dozwolonych pól sortowania
            var sortMappings = new Dictionary<string, Expression<Func<Models.Item, object>>>(
                StringComparer.OrdinalIgnoreCase)
            {
                { "name", i => i.Name! },
                { "code", i => i.Code! },
                { "price", i => i.Price! },
                { "quantity", i => i.Quantity! },
                { "category", i => i.Category!.Name! },
                { "createdat", i => i.CreatedAt }
            };

            if (!sortMappings.TryGetValue(sortBy, out var sortExpression))
            {
                // Nieznane pole - domyślne sortowanie
                return query.OrderBy(i => i.Name);
            }

            return descending
                ? query.OrderByDescending(sortExpression)
                : query.OrderBy(sortExpression);
        }
    }
}
```

### 2.5. Aktualizacja kontrolera

**Controllers/ItemsController.cs:**

```csharp
[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Pobiera paginowaną listę produktów z filtrami
    /// </summary>
    /// <param name="query">Parametry paginacji i filtrowania</param>
    /// <returns>Paginowana lista produktów</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<ItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllItemsPaginatedQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Pobiera wszystkie produkty bez paginacji (dla dropdownów)
    /// </summary>
    [HttpGet("all")]
    [ProducesResponseType(typeof(List<ItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSimple()
    {
        var query = new GetAllItemsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // ... pozostałe endpointy ...
}
```

---

## CZĘŚĆ 3: Paginacja w React Native (30 minut)

### 3.1. Typy dla paginacji

**src/types/pagination.ts:**

```typescript
/**
 * Odpowiedź paginowana z API
 */
export interface PaginatedList<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

/**
 * Parametry żądania paginowanego
 */
export interface PaginatedRequest {
  pageNumber?: number;
  pageSize?: number;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}

/**
 * Parametry wyszukiwania produktów
 */
export interface ItemSearchParams extends PaginatedRequest {
  search?: string;
  categoryId?: number;
  unitOfMeasurementId?: number;
  minPrice?: number;
  maxPrice?: number;
  isActive?: boolean;
  inStock?: boolean;
}
```

### 3.2. Hook usePaginatedFetch

**src/hooks/usePaginatedFetch.ts:**

```typescript
import { useState, useCallback, useEffect } from 'react';
import type { PaginatedList, PaginatedRequest } from '../types/pagination';

interface UsePaginatedFetchOptions<T, P extends PaginatedRequest> {
  fetchFn: (params: P) => Promise<PaginatedList<T>>;
  initialParams?: Partial<P>;
  autoLoad?: boolean;
}

interface UsePaginatedFetchResult<T, P> {
  data: T[];
  loading: boolean;
  loadingMore: boolean;
  error: string | null;
  pageNumber: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;

  // Akcje
  loadData: (params?: Partial<P>) => Promise<void>;
  loadMore: () => Promise<void>;
  refresh: () => Promise<void>;
  goToPage: (page: number) => Promise<void>;
  updateParams: (params: Partial<P>) => void;
}

export function usePaginatedFetch<T, P extends PaginatedRequest>(
  options: UsePaginatedFetchOptions<T, P>
): UsePaginatedFetchResult<T, P> {
  const { fetchFn, initialParams = {}, autoLoad = true } = options;

  const [data, setData] = useState<T[]>([]);
  const [loading, setLoading] = useState(false);
  const [loadingMore, setLoadingMore] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [params, setParams] = useState<P>({
    pageNumber: 1,
    pageSize: 20,
    ...initialParams,
  } as P);

  const [meta, setMeta] = useState({
    totalCount: 0,
    totalPages: 0,
    hasNextPage: false,
    hasPreviousPage: false,
  });

  const loadData = useCallback(async (newParams?: Partial<P>) => {
    try {
      setLoading(true);
      setError(null);

      const mergedParams = { ...params, ...newParams, pageNumber: 1 } as P;
      setParams(mergedParams);

      const result = await fetchFn(mergedParams);

      setData(result.items);
      setMeta({
        totalCount: result.totalCount,
        totalPages: result.totalPages,
        hasNextPage: result.hasNextPage,
        hasPreviousPage: result.hasPreviousPage,
      });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Błąd ładowania');
    } finally {
      setLoading(false);
    }
  }, [fetchFn, params]);

  const loadMore = useCallback(async () => {
    if (loadingMore || !meta.hasNextPage) return;

    try {
      setLoadingMore(true);
      setError(null);

      const nextPage = params.pageNumber! + 1;
      const mergedParams = { ...params, pageNumber: nextPage } as P;
      setParams(mergedParams);

      const result = await fetchFn(mergedParams);

      setData(prev => [...prev, ...result.items]);
      setMeta({
        totalCount: result.totalCount,
        totalPages: result.totalPages,
        hasNextPage: result.hasNextPage,
        hasPreviousPage: result.hasPreviousPage,
      });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Błąd ładowania');
    } finally {
      setLoadingMore(false);
    }
  }, [fetchFn, params, loadingMore, meta.hasNextPage]);

  const refresh = useCallback(async () => {
    await loadData({ pageNumber: 1 } as Partial<P>);
  }, [loadData]);

  const goToPage = useCallback(async (page: number) => {
    if (page < 1 || page > meta.totalPages) return;
    await loadData({ pageNumber: page } as Partial<P>);
  }, [loadData, meta.totalPages]);

  const updateParams = useCallback((newParams: Partial<P>) => {
    loadData(newParams);
  }, [loadData]);

  // Auto-load przy montowaniu
  useEffect(() => {
    if (autoLoad) {
      loadData();
    }
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  return {
    data,
    loading,
    loadingMore,
    error,
    pageNumber: params.pageNumber || 1,
    ...meta,
    loadData,
    loadMore,
    refresh,
    goToPage,
    updateParams,
  };
}
```

### 3.3. Aktualizacja ApiService

**src/api/apiService.ts (fragment):**

```typescript
import type { PaginatedList, ItemSearchParams } from '../types/pagination';

// ... w klasie ApiService ...

/**
 * Pobiera paginowaną listę produktów
 */
async getItemsPaginated(params: ItemSearchParams = {}): Promise<PaginatedList<Item>> {
  const queryParams = new URLSearchParams();

  if (params.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString());
  if (params.pageSize) queryParams.append('pageSize', params.pageSize.toString());
  if (params.sortBy) queryParams.append('sortBy', params.sortBy);
  if (params.sortDirection) queryParams.append('sortDirection', params.sortDirection);
  if (params.search) queryParams.append('search', params.search);
  if (params.categoryId) queryParams.append('categoryId', params.categoryId.toString());
  if (params.minPrice) queryParams.append('minPrice', params.minPrice.toString());
  if (params.maxPrice) queryParams.append('maxPrice', params.maxPrice.toString());
  if (params.isActive !== undefined) queryParams.append('isActive', params.isActive.toString());
  if (params.inStock !== undefined) queryParams.append('inStock', params.inStock.toString());

  const queryString = queryParams.toString();
  const endpoint = `/Items${queryString ? `?${queryString}` : ''}`;

  return this.request<PaginatedList<Item>>(endpoint);
}
```

### 3.4. Komponent InfiniteItemsList

**src/screens/ItemsListScreen.tsx:**

```tsx
import React, { useState, useCallback } from 'react';
import {
  View,
  Text,
  FlatList,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  ActivityIndicator,
  RefreshControl,
} from 'react-native';
import { usePaginatedFetch } from '../hooks/usePaginatedFetch';
import apiService from '../api/apiService';
import type { Item } from '../types/models';
import type { ItemSearchParams } from '../types/pagination';

interface Props {
  navigation: any;
}

const ItemsListScreen: React.FC<Props> = ({ navigation }) => {
  const [searchText, setSearchText] = useState('');

  const {
    data: items,
    loading,
    loadingMore,
    error,
    totalCount,
    hasNextPage,
    loadMore,
    refresh,
    updateParams,
  } = usePaginatedFetch<Item, ItemSearchParams>({
    fetchFn: (params) => apiService.getItemsPaginated(params),
    initialParams: { pageSize: 20, isActive: true },
  });

  // Debounced search
  const handleSearch = useCallback((text: string) => {
    setSearchText(text);
    // Debounce search (prosty timeout)
    const timeoutId = setTimeout(() => {
      updateParams({ search: text || undefined });
    }, 500);
    return () => clearTimeout(timeoutId);
  }, [updateParams]);

  const renderItem = useCallback(({ item }: { item: Item }) => (
    <TouchableOpacity
      style={styles.itemCard}
      onPress={() => navigation.navigate('ItemDetails', { itemId: item.idItem })}
    >
      <View style={styles.itemHeader}>
        <Text style={styles.itemName}>{item.name}</Text>
        {item.code && <Text style={styles.itemCode}>{item.code}</Text>}
      </View>

      <View style={styles.itemDetails}>
        <Text style={styles.category}>{item.categoryName || 'Brak kategorii'}</Text>
        <Text style={styles.price}>{item.price?.toFixed(2)} zł</Text>
      </View>

      <View style={styles.itemFooter}>
        <Text style={styles.quantity}>
          Stan: {item.quantity} {item.unitName || 'szt'}
        </Text>
      </View>
    </TouchableOpacity>
  ), [navigation]);

  const renderFooter = useCallback(() => {
    if (!loadingMore) return null;
    return (
      <View style={styles.loadingMore}>
        <ActivityIndicator size="small" color="#007AFF" />
        <Text style={styles.loadingMoreText}>Ładowanie...</Text>
      </View>
    );
  }, [loadingMore]);

  const renderEmpty = useCallback(() => {
    if (loading) return null;
    return (
      <View style={styles.emptyContainer}>
        <Text style={styles.emptyText}>
          {searchText ? 'Brak wyników wyszukiwania' : 'Brak produktów'}
        </Text>
      </View>
    );
  }, [loading, searchText]);

  return (
    <View style={styles.container}>
      {/* Header z wyszukiwarką */}
      <View style={styles.header}>
        <View style={styles.searchContainer}>
          <TextInput
            style={styles.searchInput}
            placeholder="Szukaj produktów..."
            value={searchText}
            onChangeText={handleSearch}
            placeholderTextColor="#999"
          />
          {searchText ? (
            <TouchableOpacity
              style={styles.clearButton}
              onPress={() => handleSearch('')}
            >
              <Text style={styles.clearButtonText}>✕</Text>
            </TouchableOpacity>
          ) : null}
        </View>

        <TouchableOpacity
          style={styles.addButton}
          onPress={() => navigation.navigate('CreateItem')}
        >
          <Text style={styles.addButtonText}>+</Text>
        </TouchableOpacity>
      </View>

      {/* Info o wynikach */}
      <View style={styles.resultsInfo}>
        <Text style={styles.resultsText}>
          Znaleziono: {totalCount} produktów
        </Text>
      </View>

      {/* Error */}
      {error && (
        <View style={styles.errorContainer}>
          <Text style={styles.errorText}>{error}</Text>
          <TouchableOpacity onPress={refresh}>
            <Text style={styles.retryText}>Spróbuj ponownie</Text>
          </TouchableOpacity>
        </View>
      )}

      {/* Lista */}
      <FlatList
        data={items}
        keyExtractor={(item) => item.idItem.toString()}
        renderItem={renderItem}
        contentContainerStyle={styles.listContent}
        refreshControl={
          <RefreshControl refreshing={loading} onRefresh={refresh} />
        }
        onEndReached={loadMore}
        onEndReachedThreshold={0.5}
        ListFooterComponent={renderFooter}
        ListEmptyComponent={renderEmpty}
      />
    </View>
  );
};

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#f5f5f5' },
  header: {
    flexDirection: 'row',
    padding: 12,
    backgroundColor: '#fff',
    borderBottomWidth: 1,
    borderBottomColor: '#e0e0e0',
  },
  searchContainer: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#f0f0f0',
    borderRadius: 8,
    marginRight: 12,
  },
  searchInput: {
    flex: 1,
    padding: 10,
    fontSize: 16,
    color: '#333',
  },
  clearButton: {
    padding: 10,
  },
  clearButtonText: {
    color: '#666',
    fontSize: 16,
  },
  addButton: {
    width: 44,
    height: 44,
    backgroundColor: '#007AFF',
    borderRadius: 22,
    justifyContent: 'center',
    alignItems: 'center',
  },
  addButtonText: {
    color: '#fff',
    fontSize: 24,
    fontWeight: 'bold',
  },
  resultsInfo: {
    padding: 8,
    backgroundColor: '#e8e8e8',
  },
  resultsText: {
    fontSize: 12,
    color: '#666',
    textAlign: 'center',
  },
  listContent: {
    padding: 12,
  },
  itemCard: {
    backgroundColor: '#fff',
    padding: 16,
    borderRadius: 8,
    marginBottom: 12,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.1,
    shadowRadius: 2,
    elevation: 2,
  },
  itemHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 8,
  },
  itemName: {
    fontSize: 16,
    fontWeight: '600',
    color: '#333',
    flex: 1,
  },
  itemCode: {
    fontSize: 12,
    color: '#007AFF',
    backgroundColor: '#E3F2FD',
    paddingHorizontal: 8,
    paddingVertical: 2,
    borderRadius: 4,
  },
  itemDetails: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: 8,
  },
  category: {
    fontSize: 14,
    color: '#666',
  },
  price: {
    fontSize: 16,
    fontWeight: 'bold',
    color: '#4CAF50',
  },
  itemFooter: {
    paddingTop: 8,
    borderTopWidth: 1,
    borderTopColor: '#f0f0f0',
  },
  quantity: {
    fontSize: 12,
    color: '#999',
  },
  loadingMore: {
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: 'center',
    padding: 16,
  },
  loadingMoreText: {
    marginLeft: 8,
    color: '#666',
  },
  emptyContainer: {
    padding: 40,
    alignItems: 'center',
  },
  emptyText: {
    fontSize: 16,
    color: '#999',
  },
  errorContainer: {
    padding: 16,
    backgroundColor: '#FFEBEE',
    alignItems: 'center',
  },
  errorText: {
    color: '#C62828',
    marginBottom: 8,
  },
  retryText: {
    color: '#007AFF',
    fontWeight: '600',
  },
});

export default ItemsListScreen;
```

---

## CZĘŚĆ 4: Pola Audytowe (30 minut)

### 4.1. Interfejs IAuditable

**Models/IAuditable.cs:**

```csharp
namespace SolutionOrdersReact.Server.Models
{
    /// <summary>
    /// Interfejs dla encji z polami audytowymi
    /// </summary>
    public interface IAuditable
    {
        DateTime CreatedAt { get; set; }
        string? CreatedBy { get; set; }
        DateTime? UpdatedAt { get; set; }
        string? UpdatedBy { get; set; }
    }
}
```

### 4.2. Bazowa encja BaseEntity

**Models/BaseEntity.cs:**

```csharp
using System.ComponentModel.DataAnnotations;

namespace SolutionOrdersReact.Server.Models
{
    /// <summary>
    /// Bazowa klasa dla wszystkich encji z polami audytowymi
    /// </summary>
    public abstract class BaseEntity : IAuditable
    {
        /// <summary>
        /// Czy rekord jest aktywny (soft delete)
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Data utworzenia rekordu
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Kto utworzył rekord (email/login)
        /// </summary>
        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Data ostatniej modyfikacji
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Kto ostatnio zmodyfikował (email/login)
        /// </summary>
        [MaxLength(100)]
        public string? UpdatedBy { get; set; }
    }
}
```

### 4.3. Aktualizacja modelu Item

**Models/Item.cs:**

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolutionOrdersReact.Server.Models
{
    /// <summary>
    /// Produkt/Towar - dziedziczy z BaseEntity (pola audytowe)
    /// </summary>
    public class Item : BaseEntity
    {
        [Key]
        public int IdItem { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(50)]
        public string? Code { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Quantity { get; set; }

        // FK do Category
        public int IdCategory { get; set; }
        public virtual Category Category { get; set; } = null!;

        // FK do UnitOfMeasurement (opcjonalny)
        public int? IdUnitOfMeasurement { get; set; }
        public virtual UnitOfMeasurement? UnitOfMeasurement { get; set; }

        // Relacja do OrderItems
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
```

### 4.4. Automatyczne ustawianie pól audytowych

**Data/ApplicationDbContext.cs:**

```csharp
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Models;

namespace SolutionOrdersReact.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor? httpContextAccessor = null)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Item> Items => Set<Item>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<UnitOfMeasurement> UnitsOfMeasurement => Set<UnitOfMeasurement>();
        public DbSet<Client> Clients => Set<Client>();
        public DbSet<Worker> Workers => Set<Worker>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        /// <summary>
        /// Override SaveChangesAsync do automatycznego ustawiania pól audytowych
        /// </summary>
        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var currentUser = GetCurrentUser();

            foreach (var entry in ChangeTracker.Entries<IAuditable>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.CreatedBy = currentUser;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = now;
                        entry.Entity.UpdatedBy = currentUser;
                        // Nie nadpisuj CreatedAt/CreatedBy
                        entry.Property(nameof(IAuditable.CreatedAt)).IsModified = false;
                        entry.Property(nameof(IAuditable.CreatedBy)).IsModified = false;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Pobiera aktualnego użytkownika z HttpContext
        /// </summary>
        private string? GetCurrentUser()
        {
            // W przyszłości: pobierz z JWT Claims
            // return _httpContextAccessor?.HttpContext?.User?.Identity?.Name;

            // Na razie: zwróć stałą wartość lub null
            return _httpContextAccessor?.HttpContext?.User?.Identity?.Name
                ?? "system";
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Globalne filtry dla soft delete
            modelBuilder.Entity<Item>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<Category>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<Client>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<Order>().HasQueryFilter(e => e.IsActive);

            // Indeksy dla pól audytowych (przyspieszają sortowanie/filtrowanie)
            modelBuilder.Entity<Item>()
                .HasIndex(e => e.CreatedAt);

            modelBuilder.Entity<Item>()
                .HasIndex(e => e.UpdatedAt);

            // ... reszta konfiguracji ...
        }
    }
}
```

### 4.5. Rejestracja IHttpContextAccessor

**Program.cs:**

```csharp
// Dodaj przed AddDbContext:
builder.Services.AddHttpContextAccessor();
```

---

## CZĘŚĆ 5: Logowanie do Pliku (Serilog) (20 minut)

### 5.1. Instalacja Serilog

```powershell
Install-Package Serilog.AspNetCore
Install-Package Serilog.Sinks.File
Install-Package Serilog.Sinks.Console
Install-Package Serilog.Enrichers.Environment
Install-Package Serilog.Enrichers.Thread
```

### 5.2. Konfiguracja Serilog

**Program.cs:**

```csharp
using Serilog;
using Serilog.Events;

// Konfiguracja Serilog NA POCZĄTKU (przed builder)
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithMachineName()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] ({ThreadId}) {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();

try
{
    Log.Information("Uruchamianie aplikacji...");

    var builder = WebApplication.CreateBuilder(args);

    // Użyj Serilog zamiast domyślnego loggera
    builder.Host.UseSerilog();

    // ... reszta konfiguracji ...

    var app = builder.Build();

    // ... reszta ...

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplikacja zakończyła się nieoczekiwanie");
}
finally
{
    Log.CloseAndFlush();
}
```

### 5.3. Konfiguracja w appsettings.json

**appsettings.json:**

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "Logs/log-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      }
    ]
  }
}
```

### 5.4. Użycie loggera w Handlerach

```csharp
public class CreateItemHandler : IRequestHandler<CreateItemCommand, int>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CreateItemHandler> _logger;

    public CreateItemHandler(
        ApplicationDbContext context,
        ILogger<CreateItemHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> Handle(
        CreateItemCommand request,
        CancellationToken cancellationToken)
    {
        // Structured logging - używaj {PropertyName} zamiast string interpolation
        _logger.LogInformation(
            "Tworzenie produktu: {Name}, Kategoria: {CategoryId}, Cena: {Price}",
            request.Name,
            request.IdCategory,
            request.Price);

        var item = new Item
        {
            Name = request.Name,
            // ...
        };

        _context.Items.Add(item);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Utworzono produkt ID: {ItemId}, Nazwa: {Name}",
            item.IdItem,
            item.Name);

        return item.IdItem;
    }
}
```

---

## 📝 Zadania Praktyczne

### Zadanie 1: Paginacja zamówień
Zaimplementuj paginowany endpoint dla zamówień z filtrami: data, klient, status.

### Zadanie 2: Sortowanie wielopolowe
Dodaj możliwość sortowania po wielu polach: `sortBy=price,name&sortDirection=desc,asc`.

### Zadanie 3: Historia zmian
Stwórz tabelę `AuditLog` zapisującą wszystkie zmiany (INSERT/UPDATE/DELETE) z poprzednimi wartościami.

### Zadanie 4: Wyszukiwanie zaawansowane
Zaimplementuj Full-Text Search dla opisu produktów.

### Zadanie 5: Eksport do CSV/Excel
Dodaj endpoint `/api/items/export` generujący plik CSV z wszystkimi produktami.

---

## 🔍 Pytania Kontrolne

1. Dlaczego paginacja jest ważna dla wydajności?
2. Jak działa `AsNoTracking()` i kiedy go używać?
3. Co to są Query Filters w EF Core?
4. Jak Serilog różni się od domyślnego loggera?
5. Dlaczego używamy structured logging (`{Property}`) zamiast interpolacji?
6. Co to jest Offset vs Keyset pagination?

---

## ➡️ Następna Lekcja

**[Lekcja 10: Natywne Moduły – Permissions, Camera, Geolokalizacja](./lekcja-10-natywne-moduly.md)**

W następnej lekcji:
- AsyncStorage dla lokalnych danych
- Permissions Android/iOS
- Dostęp do kamery
- Geolokalizacja
- Tworzenie natywnych modułów

---

**Gratulacje! 🎉 Twoja aplikacja jest teraz zoptymalizowana i śledzisz wszystkie zmiany!**
