function showSection(section) {
    // Hide home and hero
    document.getElementById('home').style.display = 'none';
    document.getElementById('hero').style.display = 'none';
    
    // Hide all content sections
    const sections = document.querySelectorAll('.content-section');
    sections.forEach(s => s.classList.remove('active'));
    
    // Show selected section
    document.getElementById(section).classList.add('active');
    
    // Scroll to top
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

function showHome() {
    // Hide all content sections
    const sections = document.querySelectorAll('.content-section');
    sections.forEach(s => s.classList.remove('active'));
    
    // Show home and hero
    document.getElementById('home').style.display = 'grid';
    document.getElementById('hero').style.display = 'block';
    
    // Scroll to top
    window.scrollTo({ top: 0, behavior: 'smooth' });
}