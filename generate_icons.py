from daimon_runtime import setup_plot, save_figure
from PIL import Image, ImageDraw, ImageFont
import os

output_dir = os.path.join(os.path.dirname(__file__), "BimNinja.Installer", "Icons")
resource_dir = os.path.join(os.path.dirname(__file__), "BimNinja.SheetExporter.Addin", "Resources", "Icons")
os.makedirs(output_dir, exist_ok=True)
os.makedirs(resource_dir, exist_ok=True)

def create_app_icon(size):
    img = Image.new("RGBA", (size, size), (44, 62, 80, 255))  # Dark blue background
    draw = ImageDraw.Draw(img)
    # Draw a simple "N" shape
    pad = size // 6
    draw.polygon([(pad, pad), (pad, size-pad), (size//3, size-pad), (size//3, pad*2), (size*2//3, pad), (size*2//3, size-pad), (size-pad, size-pad), (size-pad, pad), (size*2//3, pad), (size*2//3, pad*2), (size//3, pad*3), (size//3, pad)], fill=(39, 174, 96, 255))
    return img

def create_banner(width, height):
    img = Image.new("RGB", (width, height), (44, 62, 80))
    draw = ImageDraw.Draw(img)
    try:
        font = ImageFont.truetype("arial.ttf", 24)
    except:
        font = ImageFont.load_default()
    draw.text((20, 15), "BIM Ninja - Sheet Exporter", fill=(255, 255, 255), font=font)
    try:
        font_small = ImageFont.truetype("arial.ttf", 14)
    except:
        font_small = ImageFont.load_default()
    draw.text((20, 45), "Professional Revit Addin", fill=(200, 200, 200), font=font_small)
    return img

def create_dialog(width, height):
    img = Image.new("RGB", (width, height), (236, 240, 241))
    draw = ImageDraw.Draw(img)
    # Draw a simple geometric design
    for i in range(0, width, 40):
        draw.line([(i, 0), (i, height)], fill=(189, 195, 199), width=1)
    for i in range(0, height, 40):
        draw.line([(0, i), (width, i)], fill=(189, 195, 199), width=1)
    draw.rectangle([(50, 50), (width-50, height-50)], outline=(44, 62, 80), width=3)
    try:
        font = ImageFont.truetype("arial.ttf", 32)
    except:
        font = ImageFont.load_default()
    draw.text((width//2 - 120, height//2 - 20), "BIM NINJA", fill=(44, 62, 80), font=font)
    return img

# Generate app icons
for size in [16, 32, 48, 64, 128, 256]:
    icon = create_app_icon(size)
    icon.save(os.path.join(resource_dir, f"icon{size}.png"))

# Create ICO file for installer
ico_sizes = [16, 32, 48, 64, 128, 256]
images = [create_app_icon(s).convert("RGBA") for s in ico_sizes]
images[0].save(os.path.join(output_dir, "appicon.ico"), format="ICO", sizes=[(s, s) for s in ico_sizes])

# Generate installer bitmaps
banner = create_banner(493, 58)
banner.save(os.path.join(output_dir, "banner.bmp"))

dialog = create_dialog(493, 312)
dialog.save(os.path.join(output_dir, "dialog.bmp"))

print("Icons and installer images generated successfully!")
print(f"  - {os.path.join(resource_dir, 'icon*.png')}")
print(f"  - {os.path.join(output_dir, 'appicon.ico')}")
print(f"  - {os.path.join(output_dir, 'banner.bmp')}")
print(f"  - {os.path.join(output_dir, 'dialog.bmp')}")
