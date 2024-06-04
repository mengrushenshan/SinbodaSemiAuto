import cv2  # 用于图像处理
from PIL import Image  # 用于转换tif图片为opencv可以处理的格式
import numpy as np
import os
import matplotlib.pyplot as plt  # 用于绘制示意曲线


def compute_clarity(img):
	"""
	方法1：
	基于sobel算子，计算图像梯度，并梯度的方差作为图像清晰度指标。
	"""
	# Sobel梯度函数
	sobelx = cv2.Sobel(img, cv2.CV_64F, 1, 0, ksize=3)
	sobely = cv2.Sobel(img, cv2.CV_64F, 0, 1, ksize=3)
	combine_sobel = cv2.convertScaleAbs(cv2.addWeighted(sobelx, 0.5, sobely, 0.5, 0))
	clarity = combine_sobel.var()

	return clarity


def compute_binary_mean(img):
	"""
	方法2：
	将图片自适应二值化后，计算其所有像素的平均值。
	（和计算所有高亮点的面积大小效果相似）
	"""
	_, binary = cv2.threshold(img, 40, 255, cv2.THRESH_OTSU)
	binary_mean = cv2.mean(binary)[0]

	return binary_mean


def find_focused_img(input_type, path, plot=False):
	"""
	根据输入的图片类型和路径，计算图片的清晰度和二值化平均值，绘制示意曲线
	返回清晰度最大值、二值化平均值最小值对应的图片索引
	"""
	
	# 创建一个字典来存储图片信息
	img_inf = {'file_name': [], 'clarity': [], 'binary_mean': []}

	if input_type == 'tif':
		# 打开tif文件
		tif = Image.open(path)
		# 遍历tif文件中的每一帧
		for i in range(tif.n_frames):
			tif.seek(i)
			# 将当前帧转换为OpenCV可以处理的格式
			# img = cv2.cvtColor(np.array(tif), cv2.COLOR_RGB2BGR)
			img = np.array(tif)
			img = cv2.convertScaleAbs(img)  # 将图像转换为8位
			# 显示转换后的图片
			if plot:
				cv2.imshow('Image', img)
				cv2.waitKey(1)

			# 处理&分析图像
			img = cv2.GaussianBlur(img, (3, 3), 0)  # 使用高斯滤波去噪
			clarity = compute_clarity(img)  # 计算清晰度
			binary_mean = compute_binary_mean(img)  # 计算并二值化平均值

			# 记录数据
			img_inf['binary_mean'].append(binary_mean)
			img_inf['clarity'].append(clarity)
			img_inf['file_name'].append(f'img_{i}')
			print(f'Clarity of img_{i} is: {clarity}. Binary_mean:{binary_mean}')

	elif input_type == 'png_folder':
		# 读取文件夹中所有png类型的图片名称
		files = os.listdir(path)  # 获取文件夹中的所有文件名
		img_files = []  # 创建一个空列表来存储.png结尾的文件名
		for file in files:
			if file.endswith('.png'):
				img_files.append(file)
		img_files = sorted(img_files)  # 使用sorted函数对png_files列表进行排序

		img_inf = {'file_name': img_files, 'clarity': [], 'binary_mean': []}
		# 遍历文件中的所有png，计算清晰度
		for img_file in img_files:
			img = cv2.imread(os.path.join(path, img_file), cv2.IMREAD_GRAYSCALE)  # 以灰度格式读取图片
			img = cv2.GaussianBlur(img, (3, 3), 0)  # 使用高斯滤波去噪
			# 计算清晰度
			clarity = compute_clarity(img)
			# 计算并二值化平均值
			binary_mean = compute_binary_mean(img)

			# 记录数据
			img_inf['binary_mean'].append(binary_mean)
			img_inf['clarity'].append(clarity)
			#print(f'Clarity of {img_file} is: {clarity}. Binary_mean:{binary_mean}')

			if plot:
				# 显示图片
				cv2.imshow('Image', img)
				cv2.waitKey(1)


	# 找到clarity的最大值
	max_clarity_index = np.argmax(img_inf['clarity'])
	# 找到binary_mean的最小值
	min_binary_mean_index = np.argmin(img_inf['binary_mean'])

	# 调用matplotlib绘制示意曲线
	if plot:
		# 对'clarity'进行归一化
		clarity = np.array(img_inf['clarity'])
		clarity_normalized = (clarity - np.min(clarity)) / (np.max(clarity) - np.min(clarity))
		# 对'binary_mean'进行归一化
		binary_mean = np.array(img_inf['binary_mean'])
		binary_mean_normalized = (binary_mean - np.min(binary_mean)) / (np.max(binary_mean) - np.min(binary_mean))

		# 创建一个新的plt图形
		plt.figure()
		# 绘制归一化后的'clarity'
		plt.plot(img_inf['file_name'], clarity_normalized)
		# 在'clarity'的最大值处标记点
		plt.scatter(img_inf['file_name'][max_clarity_index], clarity_normalized[max_clarity_index], color='red', s=20)

		# 绘制归一化后的'binary_mean'
		plt.plot(img_inf['file_name'], binary_mean_normalized)
		# 在'binary_mean'的最大值处标记点
		plt.scatter(img_inf['file_name'][min_binary_mean_index], binary_mean_normalized[min_binary_mean_index],
		            color='blue', s=20)
		# 设置图形的标题和坐标轴标签
		plt.ylabel('Normalized Value')
		# 显示图形
		plt.show()

	return max_clarity_index, min_binary_mean_index


if __name__ == '__main__':
	# 使用示例：
	example_png_folder = r'./png_folder'  # png文件夹路径
	example_tif = r'./tif/1.tif'  # tif文件路径

	max_clarity_index, min_binary_mean_index = find_focused_img(input_type='tif', path=example_tif, plot=True)
	# max_clarity_index, min_binary_mean_index = find_focused_img(input_type='png_folder', path=example_png_folder, plot=True)

	print(f'The img_index with MAX clarity: {max_clarity_index}')
	print(f'The img_index with MIN binary mean: {min_binary_mean_index}')

