// --- addProduct.js ---
requireAdmin();

const form = document.getElementById("createProductForm");

form.addEventListener("submit", async function (event) {
  event.preventDefault();

  const submitBtn = document.getElementById("submitBtn");
  submitBtn.innerText = "Saving...";
  submitBtn.disabled = true;

  const newProduct = {
    productName: document.getElementById("productName").value,
    stock: parseInt(document.getElementById("productStock").value),
    price: parseFloat(document.getElementById("productPrice").value),
    imageUrl: document.getElementById("productImage").value,
    categoryId: parseInt(document.getElementById("productCategory").value),
  };

  const token = getToken();

  try {
    const response = await fetch(`${API_BASE_URL}/Product/add_product`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(newProduct),
    });

    if (response.ok) {
      alert("Product added successfully!");
      window.location.href = "adminDashboard.html";
    } else {
      const errorText = await response.text();
      alert("Failed to add product: " + errorText);
    }
  } catch (error) {
    console.error("Server is unreachable.", error);
  } finally {
    submitBtn.innerText = "Save Product";
    submitBtn.disabled = false;
  }
});

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

loadCategories();
