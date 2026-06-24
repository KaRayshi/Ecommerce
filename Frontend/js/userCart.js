// --- userCart.js ---

// Add this to the very top of userCart.js!
let currentCartData = [];

async function loadCart() {
  const token = getToken();

  if (!token) {
    document.getElementById("cartItemsContainer").innerHTML =
      "<h3>Please log in to view your cart.</h3>";
    return;
  }

  try {
    const response = await fetch(`${API_BASE_URL}/Cart/view_cart`, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    if (response.ok) {
      const data = await response.json();
      currentCartData = data;

      // 1. NEW: Grab the currently checked boxes right BEFORE we redraw!
      const checkedBoxes = document.querySelectorAll(".item-checkbox:checked");
      const checkedIds = Array.from(checkedBoxes).map((box) =>
        parseInt(box.value),
      );

      // 2. NEW: Pass those saved IDs into the display function
      displayCart(data, checkedIds);
      recalculateTotals();
    } else {
      console.error("Failed to load cart. Status:", response.status);
    }
  } catch (error) {
    console.error("Server is offline or unreachable.", error);
  }
}

// 1. NEW: Add the checkedIds parameter with a default empty array
function displayCart(cartArray, checkedIds = []) {
  const container = document.getElementById("cartItemsContainer");
  container.innerHTML = "";

  if (cartArray.length == 0) {
    container.innerHTML = "<h3>No products found in cart.</h3>";
    return;
  }

  cartArray.forEach(function (cartItem) {
    // 2. NEW: If this item's ID is in our saved list, save the word "checked", otherwise save empty string
    const isChecked = checkedIds.includes(cartItem.id) ? "checked" : "";

    // 3. NEW: Inject ${isChecked} right into the input tag!
    const itemHTML = `
      <div class="cart-item cart-card">
      <input type="checkbox" class="item-checkbox" value="${cartItem.id}" ${isChecked} onchange="recalculateTotals()">
          <img src="${cartItem.imageUrl}" alt="${cartItem.productName}" />
          
          <div class="item-details">
              <h3>${cartItem.productName}</h3>
              <p class="item-price">₱${cartItem.productPrice}</p>
          </div>

          <div class="qty-controls">
              <button class="qty-btn" onclick="updateQuantity(${cartItem.id}, ${cartItem.quantity - 1})">-</button>
              <span>${cartItem.quantity}</span>
              <button class="qty-btn" onclick="updateQuantity(${cartItem.id}, ${cartItem.quantity + 1})">+</button>
          </div>

          <p style="font-weight: bold; margin-left: 20px;">₱${cartItem.totalPrice}</p>

          <button class="delete-item-btn" onclick= "deleteItem(${cartItem.id})">X</button>
      </div>`;
    container.innerHTML += itemHTML;
  });
}

async function updateQuantity(itemId, newQuantity) {
  if (newQuantity < 1) {
    alert(
      "Quantity cannot be less than 1. Use the 'X' button to remove the item.",
    );
    return;
  }

  const token = getToken();

  try {
    const response = await fetch(
      `${API_BASE_URL}/Cart/update_quantity/${itemId}`,
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          quantity: newQuantity,
        }),
      },
    );

    if (response.ok) {
      loadCart();
    } else {
      const errorText = await response.text();
      console.error(
        "Failed to update cart. Status:",
        response.status,
        errorText,
      );
    }
  } catch (error) {
    console.error("Server is offline or unreachable.", error);
  }
}

function recalculateTotals() {
  let grandTotal = 0;
  let itemTotal = 0;

  const checkedBoxes = document.querySelectorAll(".item-checkbox:checked");

  checkedBoxes.forEach(function (box) {
    const itemId = parseInt(box.value);

    const item = currentCartData.find((i) => i.id === itemId);

    if (item) {
      grandTotal += item.totalPrice;
      itemTotal += item.quantity;
    }
  });

  document.getElementById("totalItemsCount").innerText = itemTotal;
  document.getElementById("totalPriceAmount").innerText =
    `₱${grandTotal.toFixed(2)}`;
}

async function checkout() {
  const checkedBoxes = document.querySelectorAll(".item-checkbox:checked");

  const isConfirmed = confirm("Confirm Checkout");
  if (!isConfirmed) return;

  if (checkedBoxes.length == 0) {
    alert("Please select at least one item to checkout.");
    return;
  }

  const selectedIds = Array.from(checkedBoxes).map((box) =>
    parseInt(box.value),
  );

  const token = getToken();

  try {
    const response = await fetch(`${API_BASE_URL}/Orders/checkout`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({ selectedCartItemIds: selectedIds }),
    });
    if (response.ok) {
      alert("Checkout successful! Your order has been placed.");
      loadCart(); // Reload to clear the checked out items!
    } else {
      const errorText = await response.text();
      alert("Checkout failed: " + errorText);
    }
  } catch (error) {
    console.error("Server is unreachable.", error);
  }
}

async function deleteItem(itemId) {
  const isConfirmed = confirm("Are you sure to remove this item?");

  if (!isConfirmed) {
    return;
  }
  const token = getToken();

  if (!token) {
    alert("Please Log In");

    window.location.href = "login.html";
  }

  try {
    const response = await fetch(`${API_BASE_URL}/Cart/delete_item/${itemId}`, {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    if (response.ok) {
      alert("Item Deleted");
      loadCart();
    } else {
      const errorText = await response.text();
      alert("Action failed: " + errorText);
    }
  } catch (error) {
    console.error("Server is unreachable.", error);
  }
}

loadCart();
