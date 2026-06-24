// --- productDashboard.js ---
async function loadRealProducts() {
  // if (!requireAdmin()) return;

  try {
    const response = await fetch(`${API_BASE_URL}/Product/view_products`, {
      method: "GET",
    });

    if (response.ok) {
      const realProducts = await response.json();
      displayProducts(realProducts);
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

function displayProducts(productsArray) {
  const container = document.getElementById("productContainer");
  container.innerHTML = "";

  productsArray.forEach(function (product) {
    const productHTML = `
      <div class="product-card">
          <h3>${product.productName}</h3>
          <img src="${product.imageUrl}" alt="${product.productName}" />
          <p class="product-price">Price: $${product.price}</p>
          <p class="product-stock">Stock: ${product.stock}</p>
          <button class="edit-btn" onclick="window.location.href='editProduct.html?id=${product.id}'">Edit</button>
          <button class="delete-btn" onclick="deleteProduct(${product.id})">Delete</button>
      </div>
    `;
    container.innerHTML += productHTML;
  });
}

async function deleteProduct(productId) {
  const isConfirmed = confirm("Are you sure you want to delete this product?");
  if (!isConfirmed) return;

  const token = getToken();

  try {
    const response = await fetch(
      `${API_BASE_URL}/Product/delete_product/${productId}`,
      {
        method: "DELETE",
        headers: { Authorization: `Bearer ${token}` },
      },
    );

    if (response.ok) {
      alert("Product Successfully Deleted");
      loadRealProducts();
    }
  } catch (error) {
    console.error("The server is unreachable.", error);
  }
}

loadRealProducts();
