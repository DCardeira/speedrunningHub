document.addEventListener('DOMContentLoaded', function() {
    // Obtém o elemento de input que será usado para filtrar a lista de jogos
    const gameFilterInput = document.getElementById('gameFilter');
    // Obtém o elemento que contém a lista de jogos
    const gameList = document.getElementById('gameList');
    // Cria um array dos elementos 'li' que representam os jogos na lista
    const games = Array.from(gameList.getElementsByTagName('li'));

    // Adiciona um listener ao input que é usado sempre que o valor do input é alterado
    gameFilterInput.addEventListener('input', function() {
        // Obtém o valor do input e converte para minúsculas para facilitar a comparação
        const filterValue = gameFilterInput.value.toLowerCase();
        // Itera sobre cada jogo na lista
        games.forEach(function(game) {
            // Obtém o texto do jogo e converte para minúsculas
            const gameText = game.textContent.toLowerCase();
            // Verifica se o texto do jogo inclui o valor do filtro
            if (gameText.includes(filterValue)) {
                // Se incluir, define o estilo 'display' do jogo como padrão ('') para garantir que ele esteja visível
                game.style.display = '';
            } else {
                // Se não incluir, define o estilo 'display' do jogo como 'none' para escondê-lo
                game.style.display = 'none';
            }
        });
    });
});

document.addEventListener('DOMContentLoaded', function() {
    const galleryImages = [
        'images/sm64_1.jpg',
        'images/sm64_2.jpg',
        'images/sm64_3.jpg',
        'images/sm64_4.jpg',
        'images/sm64_5.jpg'
    ];

    const galleryContainer = document.getElementById('gallery');
    galleryImages.forEach(imageSrc => {
        const img = document.createElement('img');
        img.src = imageSrc;
        img.alt = 'Super Mario 64 Screenshot';
        galleryContainer.appendChild(img);
    });

})

document.addEventListener('DOMContentLoaded', function() {
    const galleryImage = [
        'images/mine_1.jpg',
        'images/mine_2.jpg',
        'images/mine_3.jpg',
        'images/mine_4.jpg',
        'images/mine_5.jpg'
    ];

    const galleryContainer = document.getElementById('gallery2');
    galleryImage.forEach(imageSrc => {
        const img = document.createElement('img');
        img.src = imageSrc;
        img.alt = 'Minecraft Screenshot';
        galleryContainer.appendChild(img);
    });

})