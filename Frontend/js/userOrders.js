// --- userOrders.js ---

async function loadMyOrders() {
  const token = getToken();

  if (!token) {
    document.getElementById("userOrdersContainer").innerHTML =
      "<h3>Please log in to view your orders.</h3>";
    return;
  }

  try {
    const response = await fetch(`${API_BASE_URL}/Orders/view_user_orders`, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
      },
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

function displayOrder(ordersArray) {
  const orderContainer = document.getElementById("userOrdersContainer");
  orderContainer.innerHTML = "";

  if (ordersArray.length === 0) {
    orderContainer.innerHTML = "<h3>You haven't placed any orders yet.</h3>";
    return;
  }

  ordersArray.forEach(function (order) {
    const orderDate = new Date(order.dateOrdered).toLocaleDateString("en-US", {
      year: "numeric",
      month: "long",
      day: "numeric",
    });

    let itemsHTML = "";
    order.orderItems.forEach(function (item) {
      itemsHTML += `
        <div class="ordered-item">
            <img src="${item.imageUrl}" alt="${item.productName}" />
            
            <div class="item-text">
                <h4>${item.productName}</h4>
                <p>Qty: ${item.quantity} | Price: ₱${item.historicalPrice.toFixed(2)}</p>
            </div>
            
            <div style="margin-left: auto; font-weight: bold; color: #333;">
                ₱${item.totalPrice.toFixed(2)}
            </div>
        </div>
      `;
    });

    const orderHTML = `
      <div class="order-card">
          <div class="order-card-header">
              <div class="header-group">
                  <p>Order Placed</p>
                  <h4>${orderDate}</h4>
              </div>
              <div class="header-group">
                  <p>Total</p>
                  <h4>₱${order.totalPrice.toFixed(2)}</h4>
              </div>
          </div>
          
          <div class="order-card-body">
              ${itemsHTML}
          </div>
      </div>
    `;
    orderContainer.innerHTML += orderHTML;
  });
}

loadMyOrders();
