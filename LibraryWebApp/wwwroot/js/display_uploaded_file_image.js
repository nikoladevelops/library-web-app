function setupImagePreview(fileInputId, imgTagId) {
    const fileInput = document.getElementById(fileInputId);
    const imgTag = document.getElementById(imgTagId);

    if (!fileInput || !imgTag) {
        console.error(`Invalid IDs provided. Make sure #${fileInputId} and #${imgTagId} exist.`);
        return;
    }

    fileInput.addEventListener('change', function (event) {
        const file = event.target.files[0]; // Get the first selected file
        if (file && file.type.startsWith('image/')) {
            const reader = new FileReader();

            reader.onload = function (e) {
                imgTag.src = e.target.result; // Update the image src
            };

            reader.readAsDataURL(file); // Read the file as a data URL
        } else {
            alert('Please upload a valid image file.');
        }
    });
}