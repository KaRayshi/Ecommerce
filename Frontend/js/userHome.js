// --- userHome.js ---
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
        "<h3>No categories in the database yet.</h3>";
    } else {
      console.error("Something went wrong with the C# server.");
    }
  } catch (error) {
    console.error("The API is turned off or blocked by CORS.", error);
  }
}

function loadCategories(categoryArray) {
  const container = document.getElementById("categoryContainer");
  container.innerHTML = "";

  categoryArray.forEach(function (category) {
    const categoryHTML = `
      <div class="catBox" onclick="window.location.href='../pages/products.html?categoryId=${category.id}'" style="cursor: pointer;">
          <div class="card-image">
              <img src="${category.imageUrl}" alt="${category.name}" style="width: 100%; height: 100%; object-fit: cover;" />
          </div>
          <div class="card-label">
              ${category.name}
          </div>
      </div>
    `;
    container.innerHTML += categoryHTML;
  });
}

loadStoreCategories();
