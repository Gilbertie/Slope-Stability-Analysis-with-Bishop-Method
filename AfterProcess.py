import matplotlib.pyplot as plt
from PreProcess import *
import numpy as np

def draw_slope(c,center,slice_x,radius):
	plt.figure(1)
	ax = plt.subplot(111) # 一般都在ax中设置,不再plot中设置
	plt.title('Bishop', size=14)
	# 绘制边坡外框
	x_slope = [ [0,0],
				[0,c['A_x']],
				[c['A_x'],c['B_x']],
				[c['B_x'],c['width_bottom']],
				[c['width_bottom'],c['width_bottom']],
				[c['width_bottom'],0]]
	y_slope = [ [0,c['height_left']],
				[c['height_left'],c['height_left']],
				[c['A_y'],c['B_y']],
				[c['height_right'],c['height_right']],
				[c['height_right'],0],
				[0,0]] 
	for i in range(len(x_slope)):
		plt.plot(x_slope[i],y_slope[i], color='r')
		plt.scatter(x_slope[i],y_slope[i], color='b')

	# 绘制圆心
	plt.scatter(center[0],center[1], color='g')
	c_x = math.floor(center[0]*100)/100
	c_y = math.floor(center[1]*100)/100
	plt.text(c_x, c_y, (c_x,c_y),ha='center', va='bottom', fontsize=10)
	# 绘制圆弧（以直代曲）
	side_b = side_bottom(slice_x,center,radius)
	x_arc = [[slice_x[0],slice_x[1]]]
	y_arc = [[side_b[0],side_b[1]]]
	for i in range(len(slice_x)-1):
		x_arc.append([slice_x[i],slice_x[i+1]])
		y_arc.append([side_b[i],side_b[i+1]])
	for i in range(len(x_arc)):
		plt.plot(x_arc[i],y_arc[i], color='black')
	# 绘制半径
	x_radius = [[slice_x[0],center[0]],[slice_x[-1],center[0]]]
	y_radius = [[c['height_left'],center[1]],[side_b[-1],center[1]]]
	for i in range(len(x_radius)):
		plt.plot(x_radius[i],y_radius[i], color='black')
	# 绘制分条线
	# side_top(x,A_x,A_y,B_x,B_y,alpha_slope)
	side_t = side_top(slice_x,c['A_x'],c['A_y'],c['B_x'],c['B_y'],c['alpha_slope'])
	side_b = side_bottom(slice_x,center,radius)
	x_line = [[slice_x[0],slice_x[0]]]
	y_line = [[side_b[0],side_t[0]]]
	for i in range(len(slice_x)):
		x_line.append([slice_x[i],slice_x[i]])
		y_line.append([side_b[i],side_t[i]])
	for i in range(len(x_line)):
		plt.plot(x_line[i],y_line[i], color='y',linestyle='-')
	# 显示
	# plt.show()


def draw_equipotential_line(center_list,Fs_min_list):
	plt.figure(2)
	ax = plt.subplot(111)
	# 获取x,y 的最大最小值
	x_min = center_list[0][0]
	y_min = center_list[0][1]
	x_max = center_list[-1][0]
	y_max = center_list[-1][1]
	# 数据数目
	n = int(sqrt(len(center_list)))
	print("n",n)
	# 定义x, y
	x = np.linspace(x_min, x_max, n)
	y = np.linspace(y_min, y_max, n)
	# 生成网格数据
	X, Y = np.meshgrid(x, y)
	# 将Fs_min_list的list的格式转为Numpy的数组格式
	count = 0
	Fs_temp = []
	Fs_min_arr = []
	for each in Fs_min_list:
		Fs_temp.append(each)
		count += 1
		if count == n:
			Fs_min_arr.append(Fs_temp)
			Fs_temp = []
			count = 0
	Fs_min_arr = np.array(Fs_min_arr)
	# 填充等高线的颜色, 10是等高线分为几部分
	plt.contourf(X, Y, Fs_min_arr, 10, alpha = 0.75, cmap = plt.cm.hot)
	# 绘制等高线
	C = plt.contour(X, Y, Fs_min_arr, 10, colors = 'black')
	# 绘制等高线数据
	plt.clabel(C, inline = True, fontsize = 10)

	# 去除坐标轴
	# plt.xticks(())
	# plt.yticks(())
	plt.show()