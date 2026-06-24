// --- addCategory.js ---
requireAdmin();

const form = document.getElementById("createCategoryForm");

form.addEventListener("submit", async function (event) {
  event.preventDefault();

  const submitBtn = document.getElementById("submitBtn");
  submitBtn.innerText = "Saving...";
  submitBtn.disabled = true;

  const newCategory = {
    name: document.getElementById("categoryName").value,
    imageUrl: document.getElementById("categoryImage").value,
  };

  const token = getToken();

  try {
    const response = await fetch(`${API_BASE_URL}/Category/create`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(newCategory),
    });

    if (response.ok) {
      alert("Category added successfully!");
      window.location.href = "adminDashboard.html";
    } else {
      const errorText = await response.text();
      alert("Failed to add category: " + errorText);
    }
  } catch (error) {
    console.error("Server is unreachable.", error);
  } finally {
    submitBtn.innerText = "Save Category";
    submitBtn.disabled = false;
  }
});
