class NotificationService
{
    public IEnumerable<Notification> FilterAndSort(
        IEnumerable<Notification> notifications,
        NotificationFilterOptions options
    )
    {
        if (notifications is null)
            throw new ArgumentNullException();

        if (options is null)
            throw new ArgumentNullException();

        var query = notifications.AsEnumerable();

        if(options.IsRead.HasValue)
            query = query.Where(q => q.IsRead == options.IsRead.Value);

        if (options.Types is not null && options.Types.Any())
            query = query.Where(q => options.Types.Contains(q.Type));

        if(!string.IsNullOrWhiteSpace(options.SearchText))
        {
            string searchText = options.SearchText.ToLower();
            query = query.Where(q => q.Title is not null && q.Title.ToLower().Contains(searchText)
                                || q.Content is not null && q.Content.ToLower().Contains(searchText));
        }

        if (options.MinPriority.HasValue)
            query = query.Where(q => q.Priority >= options.MinPriority.Value);

        if(options.SortBy.HasValue)
        {
            IOrderedEnumerable<Notification> orderedQuery;

            switch (options.SortBy.Value)
            {
                case SortNotificationBy.Date:
                    orderedQuery = options.Descending ? query.OrderByDescending(q => q.CreatedAt)
                                                      : query.OrderBy(q => q.CreatedAt);
                    break;

                case SortNotificationBy.Priority:
                    orderedQuery = options.Descending ? query.OrderByDescending(q => q.Priority)
                                                      : query.OrderBy(q => q.Priority);
                    break;

                case SortNotificationBy.Title:
                    orderedQuery = options.Descending ? query.OrderByDescending(q => q.Title ?? string.Empty)
                                                      : query.OrderBy(q => q.Title ?? string.Empty);
                    break;
                default:
                    orderedQuery = options.Descending ? query.OrderByDescending(q => q.Id)
                                                      : query.OrderBy(q => q.Id);
                    break;
            }

            return orderedQuery;
        }

        return query;
    }
}
