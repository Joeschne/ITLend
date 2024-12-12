window.bootstrapModalInterop = {
    showModal: (selector) => {
        const modal = document.querySelector(selector);
        if (modal) {
            const bsModal = new bootstrap.Modal(modal);
            bsModal.show();
        }
    },
    hideModal: (selector) => {
        const modal = document.querySelector(selector);
        if (modal) {
            const bsModal = bootstrap.Modal.getInstance(modal);
            bsModal.hide();
        }
    }
};

