// Namespace for all application models
namespace CursorProject.Entities
{
    /// <summary>
    /// Enumeration for order status values
    /// Defines the possible states an order can be in
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Order has been placed but not yet processed
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Order has been confirmed and is being processed
        /// </summary>
        Confirmed = 1,

        /// <summary>
        /// Order is being prepared for shipment
        /// </summary>
        Processing = 2,

        /// <summary>
        /// Order has been shipped
        /// </summary>
        Shipped = 3,

        /// <summary>
        /// Order has been delivered to the customer
        /// </summary>
        Delivered = 4,

        /// <summary>
        /// Order has been cancelled
        /// </summary>
        Cancelled = 5,

        /// <summary>
        /// Order has been returned by the customer
        /// </summary>
        Returned = 6
    }
}
