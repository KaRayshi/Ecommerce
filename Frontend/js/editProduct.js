// --- editProduct.js ---
const urlParams = new URLSearchParams(window.location.search);
const productId = urlParams.get("id");

async function loadProductDetails() {
  if (!productId) {
    alert("No Product selected to edit.");
    window.location.href = "adminDashboard.html";
    return;
  }

  try {
    const response = await fetch(
      `${API_BASE_URL}/Product/view_product/${productId}`,
    );

    if (response.ok) {
      const product = await response.json();
      document.getElementById("productName").value = product.productName;
      document.getElementById("productImage").value = product.imageUrl;
      document.getElementById("productPrice").value = product.price;
      document.getElementById("productStock").value = product.stock;
      document.getElementById("productCategory").value = product.categoryId;
    }
  } catch (error) {
    console.error("Server is offline.", error);
  }
}

async function loadCategories() {
  try {
    const response = await fetch(`${API_BASE_URL}/Category/view_Category`, {
      method: "GET",
    });

    if (response.ok) {
      const categories = await response.json();
      const categoryDropdown = document.getElementById("productCategory");
      categoryDropdown.innerHTML = `<option value="" disabled selected>-- Select a Category --</option>`;

      categories.forEach(function (category) {
        const optionHTML = `<option value="${category.id}">${category.name}</option>`;
        categoryDropdown.innerHTML += optionHTML;
      });
    }
  } catch (error) {
    console.error("Network Error: Make sure your C# server is running!", error);
  }
}

const form = document.getElementById("editProductForm");

form.addEventListener("submit", async function (event) {
  event.preventDefault();

  const updateBtn = document.getElementById("updateBtn");
  updateBtn.innerText = "Updating...";
  updateBtn.disabled = true;

  const token = getToken();

  const updatedProduct = {
    productName: document.getElementById("productName").value,
    stock: document.getElementById("productStock").value,
    price: document.getElementById("productPrice").value,
    imageUrl: document.getElementById("productImage").value,
    categoryId: document.getElementById("productCategory").value,
  };

  try {
    const response = await fetch(
      `${API_BASE_URL}/Product/update_product/${productId}`,
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(updatedProduct),
      },
    );

    if (response.ok) {
      alert("Product Updated Successfully");
      window.location.href = "adminDashboard.html";
    } else {
      const errorText = await response.text();
      alert("Failed to update: " + errorText);
    }
  } catch (error) {
    console.error("Server unreachable.", error);
  } finally {
    updateBtn.innerText = "Update Product";
    updateBtn.disabled = false;
  }
});

async function initializePage() {
  if (!requireAdmin()) return;

  await loadCategories();
  await loadProductDetails();
}

initializePage();
