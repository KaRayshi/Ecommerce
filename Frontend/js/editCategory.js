// --- editCategory.js ---
const urlParams = new URLSearchParams(window.location.search);
const categoryId = urlParams.get("id");

async function loadCategoryDetails() {
  if (!requireAdmin()) return;

  if (!categoryId) {
    alert("No category selected to edit.");
    window.location.href = "manageCategory.html";
    return;
  }

  try {
    const response = await fetch(
      `${API_BASE_URL}/Category/view_category/${categoryId}`,
    );

    if (response.ok) {
      const category = await response.json();
      document.getElementById("categoryName").value = category.name;
      document.getElementById("categoryImage").value = category.imageUrl;
    } else {
      alert("Failed to load category details.");
      window.location.href = "manageCategory.html";
    }
  } catch (error) {
    console.error("Server is offline.", error);
  }
}

const form = document.getElementById("editCategoryForm");

form.addEventListener("submit", async function (event) {
  event.preventDefault();

  const updateBtn = document.getElementById("updateBtn");
  updateBtn.innerText = "Updating...";
  updateBtn.disabled = true;

  const updatedCategory = {
    name: document.getElementById("categoryName").value,
    imageUrl: document.getElementById("categoryImage").value,
  };

  const token = getToken();

  try {
    const response = await fetch(
      `${API_BASE_URL}/Category/update_category/${categoryId}`,
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(updatedCategory),
      },
    );

    if (response.ok) {
      alert("Category updated successfully!");
      window.location.href = "manageCategory.html";
    } else {
      const errorText = await response.text();
      alert("Failed to update: " + errorText);
    }
  } catch (error) {
    console.error("Server unreachable.", error);
  } finally {
    updateBtn.innerText = "Update Category";
    updateBtn.disabled = false;
  }
});

loadCategoryDetails();
