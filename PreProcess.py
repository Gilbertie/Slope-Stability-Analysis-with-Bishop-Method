import math
import numpy as np 
from sympy import *

# 几何关系、各点坐标、参数值
def coordinate(H,alpha_slope,c_effective,phi_effective,N):
	# 根据H推算的土坡尺寸
	height_left = 3*H
	height_right = 2*H
	height_slope = abs(height_left-height_right)
	width_top = 3*H
	width_bottom = 6*H+H/math.tan(alpha_slope/180*math.pi)
	# 根据H值计算出A、B点坐标
	A_x = width_top
	A_y = height_left
	B_x = A_x + height_slope/math.tan(alpha_slope/180*math.pi)
	B_y = height_right
	phi_effective = phi_effective/180*math.pi
	# 以字典的形式返回值
	return { 'height_left':height_left,
			 'height_right':height_right,
			 'height_slope':height_slope,
			 'alpha_slope':alpha_slope,
			 'width_top':width_top,
			 'width_bottom':width_bottom,
			 'A_x':A_x,'A_y':A_y,'B_x':B_x,'B_y':B_y,
			 'c_effective':c_effective,
			 'phi_effective':phi_effective,
			 'N':N}

# 根据设置的圆心数量得出各圆心位置
def circle_center(H,A_x,A_y,seed):
	side_num = seed
	delta = 2*H/(side_num-1)
	center = []
	for i in range(side_num):
		for j in range(side_num):
			center.append([A_x+delta*i+0.05*H,A_y+delta*j+0.05*H])
	# center = [[20,15]]
	return center

# 根据圆心坐标 计算OB长度为参考半径
def circle_radius(center,B_x,B_y):
	return math.sqrt((B_x-center[0])**2+(B_y-center[1])**2)

# 上边界的分段函数表达式
def side_top(x,A_x,A_y,B_x,B_y,alpha_slope):
	y_top = []
	for i in x:
		if i <= A_x:
			y = A_y
		elif i > A_x and i <= B_x:
			y = math.tan((180-alpha_slope)/180*math.pi)*(i-A_x)+A_y
		elif i > B_x:
			y = B_y
		y_top.append(y)
	return y_top

# 圆弧与左侧坡上地面的交点 横坐标值
def point_left(center,radius,A_y): # center 是 [x,y]
	a = center[0] # 圆 (x-a)**2+(y-b)** = r**2
	b = center[1]
	r = radius
	y = A_y
	x = Symbol('x')
	return  min(solve((x-a)**2+(y-b)**2-r**2,x))

# 圆弧与右侧坡下地面的交点 横坐标值
def point_right(center,radius,A_x,A_y,B_x,B_y,bottom_or_slope): # center 是 [x,y]
	a = center[0] # 圆 (x-a)**2+(y-b)**2 = r**2
	b = center[1]
	r = radius
	if bottom_or_slope == 'bottom':
		y = B_y
		x = Symbol('x')
		ans = max(solve((x-a)**2+(y-b)**2-r**2,x))
	else:
		x = Symbol('x')
		try:
			ans = max(solve((x-a)**2+((B_y-A_y)*(x-A_x)/(B_x-A_x)+A_y-b)**2-r**2,x))
		except:
			ans = -1 
	return  ans

# 分条的宽度
def slice_width(point_left,point_right,N):
	width = abs(point_right-point_left)
	return width / N

# 分条的x坐标 维度是N+1
def slice_x_axis(point_left,point_right,N):
	x = []
	single_width = slice_width(point_left,point_right,N)
	temp = point_left
	for i in range(N+1):
		x.append(temp)
		temp += single_width
	return x


# 圆弧分段交点 维度是N+1
def side_bottom(slice_x,center,radius):
	a = center[0] # 圆 (x-a)**2+(y-b)**2 = r**2
	b = center[1]
	r = radius
	side_bottom = []
	for x in slice_x:
		y = Symbol('y')
		side_bottom.append(min(solve((x-a)**2+(y-b)**2-r**2,y)))
	return  side_bottom

# 分条中点与圆心点的夹角 维度是N
def slice_theta(slice_x,center,side_b): 
	slice_mid_x = []
	slice_mid_y = []
	slice_theta = []
	for i in range(len(slice_x)-1):
		slice_mid_x.append((slice_x[i]+slice_x[i+1])/2)
		slice_mid_y.append((side_b[i]+side_b[i+1])/2)
	for i in range(len(slice_mid_x)):
		theta = math.atan((center[0]-slice_mid_x[i])/(center[1]-slice_mid_y[i]))
		slice_theta.append(theta)
	return slice_theta # 弧度制


# 计算面积 将四边形分成两个三角形，根据海伦公式
def calcArea(x1,y1,x2,y2,x3,y3,x4,y4): 
	def calcDistance(x1,y1,x2,y2):
		return math.sqrt((x1-x2)**2+(y1-y2)**2)
	d12 = calcDistance(x1, y1, x2, y2) 
	d23 = calcDistance(x2, y2, x3, y3) 
	d34 = calcDistance(x3, y3, x4, y4) 
	d41 = calcDistance(x4, y4, x1, y1) 
	d24 = calcDistance(x2, y2, x4, y4) 
	k1 = (d12+d41+d24)/2 
	k2 = (d23+d34+d24)/2 
	s1 = math.sqrt((k1*(k1-d12)*(k1-d41)*(k1-d24)))
	s2 = math.sqrt((k2*(k2-d23)*(k2-d34)*(k2-d24)))
	s = s1+s2 
	return s

# 分条的重度 已经*了gamma 维度是N
def slice_W(slice_x,side_t,side_b,gamma): 
	slice_W = []
	for i in range(len(slice_x)-1):
		W = calcArea(slice_x[i],side_b[i],slice_x[i],side_t[i],slice_x[i+1],side_t[i+1],slice_x[i+1],side_b[i+1])*gamma
		slice_W.append(W)
	return slice_W
		