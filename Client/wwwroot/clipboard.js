function copyToClipboard(text) {
    navigator.clipboard.writeText(text)
        .then(() => {
            console.log("Text copied!");
        })
        .catch(err => {
            // Handle potential errors
            console.error('Could not copy text: ', err);
        });
}