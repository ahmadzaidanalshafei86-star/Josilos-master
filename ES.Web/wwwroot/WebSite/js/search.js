const itemsPerPage = 5;
let currentPage = 1;

function highlightQuery(text, query) {
    if (!text || !query) return text;

    const queryWords = query.split(/\s+/).filter(word => word).map(word => word.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'));
    if (queryWords.length === 0) return text;

    const pattern = new RegExp(`(${queryWords.join('|')})`, 'gi');
    return text.replace(pattern, '<span style="background-color: yellow;">$1</span>');
}

function renderResults(page) {
    const start = (page - 1) * itemsPerPage;
    const end = start + itemsPerPage;
    const resultsContainer = document.getElementById('search-results');
    resultsContainer.innerHTML = '';

    const paginatedResults = searchResults.slice(start, end);
    paginatedResults.forEach(result => {
        const html = result.Description || '';
        const plainText = html.replace(/<[^>]+>/g, '');
        const words = plainText.split(/\s+/).filter(word => word);
        const preview = words.slice(0, 50).join(' ') + (words.length > 50 ? ' ...' : '');
        const highlightedTitle = highlightQuery(result.Title, query);
        const highlightedDescription = highlightQuery(preview, query);

        const categoryLink = result.Type === 'Page'
            ? `/categories/${result.CategoryName.toLowerCase().replace(/[^\w]+/g, '-')}`
            : result.Url;

        const resultHtml = `
            <div>
                <div class="path-search">
                    <ol class="breadcrumb search-breadcrumb">
                        <li><a href="/">Home</a></li>
                        ${result.Type === 'Page' ? `<li><a href="${categoryLink}">${result.CategoryName}</a></li>` : ''}
                        <li class="active"><span class="highlight-text">${highlightedTitle}</span></li>
                    </ol>
                </div>
                <h4><a href="${result.Url}" class="highlight-text">${highlightedTitle}</a></h4>
                <p class="highlight-text">${highlightedDescription}</p>
            </div>
            <hr />
        `;
        resultsContainer.innerHTML += resultHtml;
    });
}

function renderPagination() {
    const totalPages = Math.ceil(searchResults.length / itemsPerPage);
    const paginationContainer = document.getElementById('pagination');
    paginationContainer.innerHTML = '';

    paginationContainer.innerHTML += `
        <li class="${currentPage === 1 ? 'disabled' : ''}">
            <a href="#" onclick="changePage(${currentPage - 1}); return false;" aria-label="Previous">
                <span aria-hidden="true">«</span>
            </a>
        </li>
    `;

    for (let i = 1; i <= totalPages; i++) {
        paginationContainer.innerHTML += `
            <li class="${i === currentPage ? 'active' : ''}">
                <a href="#" onclick="changePage(${i}); return false;">${i}</a>
            </li>
        `;
    }

    paginationContainer.innerHTML += `
        <li class="${currentPage === totalPages ? 'disabled' : ''}">
            <a href="#" onclick="changePage(${currentPage + 1}); return false;" aria-label="Next">
                <span aria-hidden="true">»</span>
            </a>
        </li>
    `;
}

function changePage(page) {
    if (page < 1 || page > Math.ceil(searchResults.length / itemsPerPage)) return;
    currentPage = page;
    renderResults(currentPage);
    renderPagination();
}

function updateSortOrder(baseUrl, queryParam) {
    const sortOrder = document.getElementById('sortOrder').value;
    window.location.href = `${baseUrl}?query=${encodeURIComponent(queryParam)}&sortOrder=${sortOrder}`;
}

document.addEventListener('DOMContentLoaded', () => {
    if (searchResults.length > 0) {
        renderResults(currentPage);
        renderPagination();
    }
});
