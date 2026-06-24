// --- categoryDashboard.js ---
async function loadRealCategory() {
  if (!requireAdmin()) return;

  try {
    const response = await fetch(`${API_BASE_URL}/Category/view_Category`);

    if (response.ok) {
      const realCategories = await response.json();
      displayCategories(realCategories);
    } else if (response.status === 404) {
      document.getElementById("categoryContainer").innerHTML =
        "<h3>No categories in the database yet.</h3>";
    } else {
      console.error("Something went wrong with the C# server.");
    }
  } catch (error) {
    console.error("The API is turned off or blocked by CORS.", error);
  }
}

function displayCategories(categoriesArray) {
  const container = document.getElementById("categoryContainer");
  container.innerHTML = "";

  categoriesArray.forEach(function (category) {
    const categoryHTML = `
      <div class="product-card">
          <h3>${category.name}</h3>
          <img src="${category.imageUrl}" alt="${category.name}" />
          <button class="edit-btn" onclick="window.location.href='editCategory.html?id=${category.id}'">Edit</button>
          <button class="delete-btn" onclick="deleteCategory(${category.id})">Delete</button>
      </div>
    `;
    container.innerHTML += categoryHTML;
  });
}

async function deleteCategory(categoryId) {
  const isConfirmed = confirm(
    "Are you sure you want to delete this category? This cannot be undone.",
  );
  if (!isConfirmed) return;

  const token = getToken();

  try {
    const response = await fetch(
      `${API_BASE_URL}/Category/Delete_Category/${categoryId}`,
      {
        method: "DELETE",
        headers: { Authorization: `Bearer ${token}` },
      },
    );

    if (response.ok) {
      alert("Category deleted successfully!");
      loadRealCategory();
    } else {
      const errorText = await response.text();
      alert("Failed to delete category: " + errorText);
    }
  } catch (error) {
    console.error("The server is unreachable.", error);
  }
}

loadRealCategory();
