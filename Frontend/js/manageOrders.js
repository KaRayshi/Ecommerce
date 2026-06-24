async function loadAllOrder() {
  if (!requireAdmin()) return;

  const token = getToken();

  if (!token) {
    document.getElementById("userOrdersContainer").innerHTML =
      "<h3>Please log in to view your orders.</h3>";
    return;
  }

  try {
    const response = await fetch(`${API_BASE_URL}/Orders/view_all_orders`, {
      //   headers: {
      //     Authorization: `Bearer ${token}`,
      //   },
    });

    if (response.ok) {
      const data = await response.json();

      displayOrder(data);
    } else {
      console.error("Failed to load orders. Status:", response.status);
    }
  } catch (error) {
    console.error("Server is offline or unreachable.", error);
  }
}

function displayOrder(orderArray) {
  const container = document.getElementById("adminOrdersContainer");

  // Add the wrapper class from your CSS!
  container.className = "orders-list-wrapper";
  container.innerHTML = "";

  if (orderArray.length === 0) {
    container.innerHTML = "<h3>No orders have been placed yet.</h3>";
    return;
  }

  orderArray.forEach(function (order) {
    const orderDate = new Date(order.dateOrdered).toLocaleDateString("en-US", {
      year: "numeric",
      month: "short", // Changed to "short" (e.g., Jun 21) to save space in the row!
      day: "numeric",
    });

    // Build the row exactly as your CSS expects it
    const orderHTML = `
      <div class="order-row">
          <div class="order-info">
              <h3>Order #${order.id} - ${order.customerName}</h3>
              
              <p class="order-details">
                  Placed: ${orderDate} &nbsp;|&nbsp; 
                  <strong>Total: ₱${order.totalPrice.toFixed(2)}</strong> &nbsp;|&nbsp; 
                  Items: ${order.orderItems.length}
              </p>
              
          </div>
          
          <div class="order-actions">
              <button class="view-btn" onclick="window.location.href='adminUserOrderDetail.html?id=${order.id}'">View Details</button>

          </div>
      </div>
    `;

    container.innerHTML += orderHTML;
  });
}

loadAllOrder();
