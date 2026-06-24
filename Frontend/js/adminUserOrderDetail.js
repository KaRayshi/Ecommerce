const urlParams = new URLSearchParams(window.location.search);
const orderId = urlParams.get("id");

async function loadOrderDetail() {
  if (!orderId) {
    document.querySelector(".order-detail-main").innerHTML =
      "<h2>No Order ID provided in the URL!</h2>";
    return;
  }

  const token = getToken();

  try {
    const response = await fetch(
      `${API_BASE_URL}/Orders/view_order/${orderId}`,
      {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      },
    );

    if (response.ok) {
      const orderData = await response.json();
      displayOrderDetail(orderData);
    } else {
      document.getElementById("detailItemsContainer").innerHTML =
        "<p>Error loading order.</p>";
    }
  } catch (error) {
    console.error("Server unreachable", error);
  }
}

function displayOrderDetail(order) {
  const orderDate = new Date(order.dateOrdered).toLocaleDateString("en-US", {
    year: "numeric",
    month: "long",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });

  document.getElementById("displayOrderId").innerText = order.id;
  document.getElementById("displayCustomerName").innerText = order.customerName;
  document.getElementById("displayOrderDate").innerText = orderDate;
  document.getElementById("displayGrandTotal").innerText =
    `₱${order.totalPrice.toFixed(2)}`;

  const itemsContainer = document.getElementById("detailItemsContainer");
  itemsContainer.innerHTML = "";

  if (order.orderItems.length === 0) {
    itemsContainer.innerHTML =
      "<p>This is an old order with no items (Archived).</p>";
    return;
  }

  order.orderItems.forEach((item) => {
    const itemHTML = `
        <div class="detail-item-row">
            <img src="${item.imageUrl || "../images/shopping.jpg"}" alt="${item.productName}">
            <div class="item-text">
                <h4>${item.productName}</h4>
                <p>₱${item.historicalPrice.toFixed(2)} x ${item.quantity}</p>
            </div>
            <div class="item-total">
                ₱${item.totalPrice.toFixed(2)}
            </div>
        </div>
        `;
    itemsContainer.innerHTML += itemHTML;
  });
}

loadOrderDetail();
