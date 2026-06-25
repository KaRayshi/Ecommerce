// --- userProduct.js ---
async function loadStoreCategories() {
  try {
    const response = await fetch(`${API_BASE_URL}/Category/view_Category`, {
      method: "GET",
    });

    if (response.ok) {
      const catData = await response.json();
      loadCategories(catData);
    } else if (response.status === 404) {
      document.getElementById("categoryContainer").innerHTML =
        "<h3>No products in the database yet.</h3>";
    } else {
      console.error("Something went wrong with the C# server.");
    }
  } catch (error) {
    console.error("The API is turned off or blocked by CORS.", error);
  }
}

function loadCategories(categoryArray) {
  const container = document.getElementById("categoryBox");
  let finalHTML = `<a href="../pages/products.html">All</a>`;

  categoryArray.forEach(function (category) {
    finalHTML += `
      <a href="../pages/products.html?categoryId=${category.id}">
          ${category.name}
      </a>
    `;
  });

  container.innerHTML = finalHTML;
}

async function loadRealProducts() {
  try {
    const response = await fetch(`${API_BASE_URL}/Product/view_products`, {
      method: "GET",
    });

    if (response.ok) {
      let data = await response.json();
      const urlParams = new URLSearchParams(window.location.search);
      const targetCategoryId = urlParams.get("categoryId");

      if (targetCategoryId) {
        data = data.filter((product) => product.categoryId == targetCategoryId);
      }
      displayProduct(data);
    } else if (response.status === 404) {
      document.getElementById("productContainer").innerHTML =
        "<h3>No products in the database yet.</h3>";
    } else {
      console.error("Something went wrong with the C# server.");
    }
  } catch (error) {
    console.error("The API is turned off or blocked by CORS.", error);
  }
}

function displayProduct(productArray) {
  const container = document.getElementById("productContainer");
  container.innerHTML = "";

  if (productArray.length === 0) {
    container.innerHTML = "<h3>No products found in this category.</h3>";
    return;
  }

  productArray.forEach(function (product) {
    const productsHTML = `
      <div class="product-card">
         <img src="${product.imageUrl}" alt="${product.productName}" />
         <h3>${product.productName}</h3>
         <p class="product-price">Price: $${product.price}</p>
         <p class="product-stock">Stock: ${product.stock}</p>
         
         <div class="qty-stepper">
             <button class="qty-btn" onclick="changeQty(${product.id}, -1, ${product.stock})">-</button>
             <input type="text" id="qty-${product.id}" class="qty-input" value="1" readonly>
             <button class="qty-btn" onclick="changeQty(${product.id}, 1, ${product.stock})">+</button>
         </div>
         
         <button class="add-to-cart-btn" onclick="addToCart(${product.id})">Add To Cart</button>
     </div>
    `;
    container.innerHTML += productsHTML;
  });
}

async function addToCart(productId) {
  const token = getToken();

  if (!token) {
    alert("Please log in to add items to your cart!");
    window.location.href = "login.html";
    return;
  }

  const slider = document.getElementById(`qty-${productId}`);
  const sliderValue = parseInt(slider.value);

  try {
    const response = await fetch(`${API_BASE_URL}/Cart/add_item`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({
        productId: productId,
        quantity: sliderValue,
      }),
    });

    if (response.ok) {
      alert("Item Added To Cart");
      loadRealProducts();
    } else {
      const errorText = await response.text();
      alert("Failed to add cart: " + errorText);
    }
  } catch (error) {
    console.error("Server is unreachable.", error);
  }
}

function changeQty(productId, changeAmount, maxStock) {
  const inputField = document.getElementById(`qty-${productId}`);
  let currentValue = parseInt(inputField.value);
  let newValue = currentValue + changeAmount;

  if (newValue >= 1 && newValue <= maxStock) {
    inputField.value = newValue;
  }
}

loadStoreCategories();
loadRealProducts();
