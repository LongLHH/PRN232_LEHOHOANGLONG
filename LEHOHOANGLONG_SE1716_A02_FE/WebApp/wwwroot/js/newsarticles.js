// NewsArticles JavaScript functions
const createModal = new bootstrap.Modal(document.getElementById('createModal'));
const editModal = new bootstrap.Modal(document.getElementById('editModal'));
let categories = [];
let tags = [];

// Load categories and tags on page load
document.addEventListener('DOMContentLoaded', async () => {
    await loadCategories();
    await loadTags();
});

async function loadCategories() {
    try {
        const response = await fetch('/Categories/GetAll');
        categories = await response.json();
        
        const createSelect = document.getElementById('createCategory');
        const editSelect = document.getElementById('editCategory');
        
        categories.forEach(cat => {
            createSelect.add(new Option(cat.categoryName, cat.categoryId));
            editSelect.add(new Option(cat.categoryName, cat.categoryId));
        });
    } catch (error) {
        console.error('Error loading categories:', error);
    }
}

async function loadTags() {
    try {
        const response = await fetch('/Tags/GetAll');
        tags = await response.json();
        
        const createSelect = document.getElementById('createTags');
        const editSelect = document.getElementById('editTags');
        
        tags.forEach(tag => {
            createSelect.add(new Option(tag.tagName, tag.tagId));
            editSelect.add(new Option(tag.tagName, tag.tagId));
        });
    } catch (error) {
        console.error('Error loading tags:', error);
    }
}

function showCreateModal() {
    document.getElementById('createForm').reset();
    document.getElementById('createStatus').checked = true;
    createModal.show();
}

async function showEditModal(id) {
    try {
        const response = await fetch(`/NewsArticles/GetById/${id}`);
        const article = await response.json();
        
        document.getElementById('editId').value = article.newsArticleId;
        document.getElementById('editTitle').value = article.newsTitle;
        document.getElementById('editContent').value = article.newsContent;
        document.getElementById('editCategory').value = article.categoryId.toString();
        document.getElementById('editStatus').checked = article.newsStatus;
        
        // Select tags - convert both to numbers for comparison
        const editTagsSelect = document.getElementById('editTags');
        const articleTagIds = article.tags.map(t => t.tagId);
        Array.from(editTagsSelect.options).forEach(option => {
            option.selected = articleTagIds.includes(parseInt(option.value));
        });
        
        editModal.show();
    } catch (error) {
        alert('Error loading article: ' + error.message);
    }
}

function confirmDelete(id, title) {
    if (confirm(`Are you sure you want to delete article: ${title}?\n\nThis action cannot be undone.`)) {
        window.location.href = `/NewsArticles/Delete/${id}`;
    }
}

document.getElementById('createForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const selectedTags = Array.from(document.getElementById('createTags').selectedOptions)
        .map(option => parseInt(option.value));
    
    const data = {
        NewsTitle: document.getElementById('createTitle').value,
        NewsContent: document.getElementById('createContent').value,
        CategoryId: parseInt(document.getElementById('createCategory').value),
        NewsStatus: document.getElementById('createStatus').checked,
        SelectedTagIds: selectedTags
    };

    console.log('Creating article with data:', data);

    try {
        const response = await fetch('/NewsArticles/Create', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        console.log('Response status:', response.status);
        const result = await response.json();
        console.log('Result:', result);
        
        if (response.ok && result.success) {
            alert(result.message || 'Article created successfully');
            location.reload();
        } else {
            alert('Error: ' + (result.message || 'Failed to create article'));
            console.error('Create failed:', result);
        }
    } catch (error) {
        alert('Error: ' + error.message);
        console.error('Create error:', error);
    }
});

document.getElementById('editForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const id = document.getElementById('editId').value;
    const selectedTags = Array.from(document.getElementById('editTags').selectedOptions)
        .map(option => parseInt(option.value));
    
    const data = {
        NewsArticleId: id,
        NewsTitle: document.getElementById('editTitle').value,
        NewsContent: document.getElementById('editContent').value,
        CategoryId: parseInt(document.getElementById('editCategory').value),
        NewsStatus: document.getElementById('editStatus').checked,
        SelectedTagIds: selectedTags
    };

    console.log('=== EDIT ARTICLE DEBUG ===');
    console.log('Article ID:', id);
    console.log('Data being sent:', JSON.stringify(data, null, 2));
    console.log('Category ID type:', typeof data.CategoryId);
    console.log('Tag IDs:', data.SelectedTagIds);

    try {
        const response = await fetch(`/NewsArticles/Edit/${id}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        console.log('Response status:', response.status);
        console.log('Response OK:', response.ok);
        
        const result = await response.json();
        console.log('Result:', JSON.stringify(result, null, 2));
        
        if (response.ok && result.success) {
            alert(result.message || 'Article updated successfully');
            location.reload();
        } else {
            const errorMsg = result.message || 'Failed to update article';
            console.error('Update failed:', errorMsg);
            alert('Error: ' + errorMsg);
        }
    } catch (error) {
        console.error('Edit error:', error);
        alert('Error: ' + error.message);
    }
});
