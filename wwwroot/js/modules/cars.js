// Car management functionality
class CarManager {
    constructor() {
        this.init();
    }

    init() {
        this.setupDeleteConfirmation();
        this.setupImageModal();
        this.setupSearch();
        this.setupFilters();
    }

    setupDeleteConfirmation() {
        document.addEventListener('click', (e) => {
            if (e.target.matches('[data-confirm-delete]')) {
                e.preventDefault();
                
                const message = e.target.dataset.confirmDelete || 'Are you sure you want to delete this car?';
                
                if (confirm(message)) {
                    // Create and submit form for DELETE request
                    const form = document.createElement('form');
                    form.method = 'POST';
                    form.action = e.target.href;
                    
                    // Add anti-forgery token
                    const token = document.querySelector('input[name="__RequestVerificationToken"]');
                    if (token) {
                        const hiddenToken = token.cloneNode();
                        form.appendChild(hiddenToken);
                    }
                    
                    document.body.appendChild(form);
                    form.submit();
                }
            }
        });
    }

    setupImageModal() {
        // Create modal for viewing car images
        const modal = document.createElement('div');
        modal.className = 'image-modal';
        modal.innerHTML = `
            <div class="modal-backdrop">
                <div class="modal-content">
                    <button class="modal-close">×</button>
                    <img class="modal-image" src="" alt="">
                </div>
            </div>
        `;
        document.body.appendChild(modal);

        const modalImage = modal.querySelector('.modal-image');
        const closeBtn = modal.querySelector('.modal-close');

        // Handle image clicks
        document.addEventListener('click', (e) => {
            if (e.target.matches('.car-image, .card-img-top')) {
                e.preventDefault();
                modalImage.src = e.target.src;
                modal.classList.add('show');
            }
        });

        // Handle modal close
        closeBtn.addEventListener('click', () => {
            modal.classList.remove('show');
        });

        modal.addEventListener('click', (e) => {
            if (e.target === modal || e.target.classList.contains('modal-backdrop')) {
                modal.classList.remove('show');
            }
        });

        // Handle escape key
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && modal.classList.contains('show')) {
                modal.classList.remove('show');
            }
        });
    }

    setupSearch() {
        const searchInput = document.querySelector('#carSearch');
        if (!searchInput) return;

        let searchTimeout;
        
        searchInput.addEventListener('input', (e) => {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                this.filterCars(e.target.value);
            }, 300);
        });
    }

    setupFilters() {
        const filterSelects = document.querySelectorAll('.car-filter');
        
        filterSelects.forEach(select => {
            select.addEventListener('change', () => {
                this.applyFilters();
            });
        });
    }

    filterCars(searchTerm) {
        const cars = document.querySelectorAll('.car-card');
        const term = searchTerm.toLowerCase();

        cars.forEach(car => {
            const brand = car.querySelector('.car-brand-model')?.textContent.toLowerCase() || '';
            const owner = car.querySelector('.car-owner')?.textContent.toLowerCase() || '';
            
            const matches = brand.includes(term) || owner.includes(term);
            car.style.display = matches ? 'block' : 'none';
        });

        this.updateResultsCount();
    }

    applyFilters() {
        const brandFilter = document.querySelector('#brandFilter')?.value || '';
        const yearFilter = document.querySelector('#yearFilter')?.value || '';
        const cars = document.querySelectorAll('.car-card');

        cars.forEach(car => {
            let show = true;

            if (brandFilter) {
                const carBrand = car.querySelector('.car-brand-model')?.textContent || '';
                show = show && carBrand.toLowerCase().includes(brandFilter.toLowerCase());
            }

            if (yearFilter) {
                const carYear = car.querySelector('.car-year')?.textContent || '';
                show = show && carYear.includes(yearFilter);
            }

            car.style.display = show ? 'block' : 'none';
        });

        this.updateResultsCount();
    }

    updateResultsCount() {
        const visibleCars = document.querySelectorAll('.car-card[style*="block"], .car-card:not([style*="none"])').length;
        const totalCars = document.querySelectorAll('.car-card').length;
        
        const counter = document.querySelector('.results-counter');
        if (counter) {
            counter.textContent = `Showing ${visibleCars} of ${totalCars} cars`;
        }
    }
}

// Initialize car manager
document.addEventListener('DOMContentLoaded', () => {
    window.carManager = new CarManager();
});